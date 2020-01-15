﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapper;
using Microsoft.Extensions.Logging;
using Squadio.BLL.Providers.Codes;
using Squadio.BLL.Providers.Users;
using Squadio.BLL.Services.Rabbit;
using Squadio.BLL.Services.Users;
using Squadio.Common.Models.Email;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.ChangePassword;
using Squadio.DAL.Repository.Companies;
using Squadio.DAL.Repository.CompaniesUsers;
using Squadio.DAL.Repository.Invites;
using Squadio.DAL.Repository.SignUp;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Invites;
using Squadio.DTO.Invites;
using Squadio.DTO.Users;

namespace Squadio.BLL.Services.Companies.Implementation
{
    public class CompanyInvitesService : ICompanyInvitesService
    {
        private readonly IInvitesRepository _repository;
        private readonly IRabbitService _rabbitService;
        private readonly ICompaniesRepository _companiesRepository;
        private readonly ICompaniesUsersRepository _companiesUsersRepository;
        private readonly ICodeProvider _codeProvider;
        private readonly IUsersService _usersService;
        private readonly IUsersProvider _usersProvider;
        private readonly ISignUpRepository _signUpRepository;
        private readonly IChangePasswordRequestRepository _changePasswordRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CompanyInvitesService> _logger;
        
        public CompanyInvitesService(IInvitesRepository repository
            , IRabbitService rabbitService
            , ICompaniesRepository companiesRepository
            , ICompaniesUsersRepository companiesUsersRepository
            , ICodeProvider codeProvider
            , IUsersService usersService
            , IUsersProvider usersProvider
            , ISignUpRepository signUpRepository
            , IChangePasswordRequestRepository changePasswordRepository
            , IMapper mapper
            , ILogger<CompanyInvitesService> logger)
        {
            _repository = repository;
            _rabbitService = rabbitService;
            _companiesRepository = companiesRepository;
            _companiesUsersRepository = companiesUsersRepository;
            _codeProvider = codeProvider;
            _usersService = usersService;
            _usersProvider = usersProvider;
            _signUpRepository = signUpRepository;
            _changePasswordRepository = changePasswordRepository;
            _mapper = mapper;
            _logger = logger;
        }
        
        
        public async Task<Response> CreateInvite(Guid companyId, Guid authorId, CreateInvitesDTO dto, bool sendInvites = true)
        {
            var companyUser = await _companiesUsersRepository.GetCompanyUser(companyId, authorId);
            if (companyUser == null || companyUser?.Status == UserStatus.Member)
            {
                return new PermissionDeniedErrorResponse<IEnumerable<InviteDTO>>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.PermissionDenied,
                        Message = ErrorMessages.Security.PermissionDenied,
                    }
                });
            }
            
            foreach (var email in dto.Emails)
            {
                var createInviteResult = await CreateInviteToCompany(
                    companyUser.CompanyId,
                    email,
                    authorId);
                if (createInviteResult != null && sendInvites)
                {
                    await SendCompanyInvite(
                        createInviteResult.Item1,
                        email,
                        companyUser.User.Name,
                        companyUser.Company.Name,
                        createInviteResult.Item2);
                }
            }

            return new Response();
        }

        public async Task<Response> CancelInvite(Guid companyId, Guid authorId, CancelInvitesDTO dto)
        {
            var companyUser = await _companiesUsersRepository.GetCompanyUser(companyId, authorId);
            if (companyUser == null || companyUser?.Status == UserStatus.Member)
            {
                return new PermissionDeniedErrorResponse<IEnumerable<InviteDTO>>(new[]
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.PermissionDenied,
                        Message = ErrorMessages.Security.PermissionDenied,
                    }
                });
            }

            await _companiesUsersRepository.DeleteCompanyUsers(companyId, dto.Emails);

            await _repository.ActivateInvites(companyId, dto.Emails);

            return new Response();
        }

        public async Task<Response> AcceptInvite(Guid userId, string code)
        {
            var invite = await _repository.GetInviteByCode(code);

            if (invite == null
                || invite?.IsActivated == true
                || invite?.Code.ToUpper() != code.ToUpper())
            {
                return new SecurityErrorResponse(new[]
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.InviteInvalid,
                        Message = ErrorMessages.Security.InviteInvalid
                    }
                });
            }

            var user = (await _usersProvider.GetById(userId)).Data;
            if (user == null)
            {
                return new BusinessConflictErrorResponse(new[]
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.UserDoesNotExists,
                        Message = ErrorMessages.Business.UserDoesNotExists
                    }
                });
            }

            if (user.Email.ToUpper() != invite.Email.ToUpper())
            {
                return new PermissionDeniedErrorResponse(new[]
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.PermissionDenied,
                        Message = ErrorMessages.Security.PermissionDenied
                    }
                });
            }

            var result = await AcceptInvite(invite.EntityId, user.Id);
            await _repository.ActivateInvite(invite.Id);

            return result;
        }
        
        public async Task<Response> AcceptInvite(Guid companyId, Guid userId)
        {
            var company = await _companiesRepository.GetById(companyId);
            if (company == null)
            {
                return new BusinessConflictErrorResponse(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Common.NotFound,
                        Message = ErrorMessages.Common.NotFound
                    }
                });
            }

            var companyUser = await _companiesUsersRepository.GetCompanyUser(company.Id, userId);
            if (companyUser == null)
                await _companiesUsersRepository.AddCompanyUser(company.Id, userId, UserStatus.Member);
            else if (companyUser.Status == UserStatus.Pending)
                await _companiesUsersRepository.ChangeStatusCompanyUser(company.Id, userId, UserStatus.Member);

            return new Response();
        }

        private async Task<Tuple<InviteModel, bool>> CreateInviteToCompany(Guid companyId, string email, Guid authorId)
        {
            var code = _codeProvider.GenerateCodeAsGuid();
            InviteModel invite = null;
            var isAlreadyRegistered = true;
            
            var user = (await _usersProvider.GetByEmail(email)).Data;
            if (user == null)
            {
                var createUserDTO = new UserCreateDTO()
                {
                    Email = email,
                    Step = RegistrationStep.New,
                    Status = UserStatus.Member
                };
                user = (await _usersService.CreateUser(createUserDTO)).Data;
                
                var signUpStep = await _signUpRepository.GetRegistrationStepByUserId(user.Id);
                if (signUpStep.Step == RegistrationStep.New)
                {
                    isAlreadyRegistered = false;
                    await _changePasswordRepository.AddRequest(user.Id, code);
                }
            
                var companyUser = await _companiesUsersRepository.GetCompanyUser(companyId, user.Id);
                if (companyUser != null)
                {
                    return null;
                }
            
                await _companiesUsersRepository.AddCompanyUser(companyId, user.Id, UserStatus.Pending);
            
                invite = new InviteModel
                {
                    Email = email,
                    IsActivated = false,
                    CreatedDate = DateTime.UtcNow,
                    Code = code,
                    EntityId = companyId,
                    EntityType = EntityType.Company,
                    CreatorId = authorId
                };
            
                invite = await _repository.CreateInvite(invite);
            }
            
            return new Tuple<InviteModel, bool>(invite, isAlreadyRegistered);
        }
        
        private async Task SendCompanyInvite(InviteModel model, string email, string authorName, string companyName, bool isAlreadyRegistered)
        {
            try
            {
                if (model != null)
                {
                    await _rabbitService.Send(new InviteToCompanyEmailModel()
                    {
                        To = email,
                        AuthorName = authorName,
                        Code = model.Code,
                        CompanyName = companyName,
                        IsAlreadyRegistered = isAlreadyRegistered
                    });
                }
                else
                {
                    await _rabbitService.Send(new AddToCompanyEmailModel()
                    {
                        To = email,
                        AuthorName = authorName,
                        CompanyName = companyName
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{email}] : {ex.Message}");
            }
        }
    }
}