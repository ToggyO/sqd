using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapper;
using Microsoft.Extensions.Logging;
using Squadio.BLL.Providers.Codes;
using Squadio.BLL.Providers.Users;
using Squadio.BLL.Services.Rabbit;
using Squadio.BLL.Services.Teams;
using Squadio.BLL.Services.Users;
using Squadio.Common.Models.Email;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Companies;
using Squadio.DAL.Repository.CompaniesUsers;
using Squadio.DAL.Repository.Invites;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;
using Squadio.Domain.Models.Invites;
using Squadio.DTO.Companies;
using Squadio.DTO.Invites;
using Squadio.DTO.Users;

namespace Squadio.BLL.Services.Companies.Implementation
{
    public class CompaniesService : ICompaniesService
    {
        private readonly ICompaniesRepository _repository;
        private readonly ICompaniesUsersRepository _companiesUsersRepository;
        private readonly ITeamsService _teamsService;
        private readonly IUsersService _usersService;
        private readonly IUsersProvider _usersProvider;
        private readonly ICodeProvider _codeProvider;
        private readonly IMapper _mapper;
        private readonly ILogger<CompaniesService> _logger;
        public CompaniesService(ICompaniesRepository repository
            , ICompaniesUsersRepository companiesUsersRepository
            , ITeamsService teamsService
            , IUsersService usersService
            , IUsersProvider usersProvider
            , ICodeProvider codeProvider
            , IMapper mapper
            , ILogger<CompaniesService> logger)
        {
            _repository = repository;
            _companiesUsersRepository = companiesUsersRepository;
            _teamsService = teamsService;
            _usersService = usersService;
            _usersProvider = usersProvider;
            _codeProvider = codeProvider;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Response> InviteUsers(Guid companyId, Guid authorId, CreateInvitesDTO dto)
        {
            var companyUsers = await _companiesUsersRepository.GetCompanyUsersByEmails(new PageModel {Page = 1, PageSize = 1000000}
                , companyId
                , dto.Emails);
            
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
            
            var existedEmails = companyUsers.Items.Select(x => x.User.Email);
            var notExistedEmails = dto.Emails.Except(existedEmails);
            foreach (var email in notExistedEmails)
            {
                var createInviteResult = await CreateInviteToCompany(
                    companyUser.CompanyId,
                    email,
                    authorId);
            }
            throw new NotImplementedException();
        }

        public async Task<Response<CompanyDTO>> Update(Guid companyId, Guid userId, CompanyUpdateDTO dto)
        {
            var companyUser = await _companiesUsersRepository.GetCompanyUser(companyId, userId);
            
            if (companyUser == null)
            {
                return new BusinessConflictErrorResponse<CompanyDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Common.NotFound,
                        Message = ErrorMessages.Common.NotFound,
                        Field = "userId"
                    }
                });
            }

            if (companyUser.Status != UserStatus.SuperAdmin)
            {
                return new PermissionDeniedErrorResponse<CompanyDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.PermissionDenied,
                        Message = ErrorMessages.Security.PermissionDenied
                    }
                }); 
            }
            
            var companyEntity = await _repository.GetById(companyId);
            
            if (companyEntity == null)
            {
                return new BusinessConflictErrorResponse<CompanyDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Common.NotFound,
                        Message = ErrorMessages.Common.NotFound,
                        Field = "companyId"
                    }
                });
            }
            
            companyEntity.Name = dto.Name;
            companyEntity.Address = dto.Address;
            
            companyEntity = await _repository.Update(companyEntity);
            
            var result = _mapper.Map<CompanyModel, CompanyDTO>(companyEntity);
            
            return new Response<CompanyDTO>
            {
                Data = result
            };
        }

        public async Task<Response> DeleteUserFromCompany(Guid companyId, Guid removeUserId, Guid currentUserId)
        {
            var currentCompanyUser = await _companiesUsersRepository.GetCompanyUser(companyId, currentUserId);

            if (currentCompanyUser == null || currentCompanyUser?.Status != UserStatus.SuperAdmin)
            {
                return new PermissionDeniedErrorResponse(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.PermissionDenied,
                        Message = ErrorMessages.Security.PermissionDenied
                    }
                }); 
            }

            await _companiesUsersRepository.DeleteCompanyUser(companyId, removeUserId);
            await _teamsService.DeleteUserFromTeamsByCompanyId(companyId, removeUserId);
            
            return new Response();
        }

        public async Task<Response<CompanyDTO>> Create(Guid userId, CompanyCreateDTO dto)
        {
            var entityCompany = new CompanyModel
            {
                Name = dto.Name,
                Address = dto.Address,
                CreatedDate = DateTime.UtcNow,
                CreatorId = userId
            };
            
            entityCompany = await _repository.Create(entityCompany);

            await _companiesUsersRepository.AddCompanyUser(entityCompany.Id, userId, UserStatus.SuperAdmin);
            
            var result = _mapper.Map<CompanyModel, CompanyDTO>(entityCompany);
            
            return new Response<CompanyDTO>
            {
                Data = result
            };
        }

        private async Task<Tuple<InviteModel, bool>> CreateInviteToCompany(Guid companyId, string email, Guid authorId)
        {
            var code = _codeProvider.GenerateCodeAsGuid();
            var isAlreadyRegistered = true;

            var user = (await _usersProvider.GetByEmail(email)).Data;
            if (user != null)
            {
                return new Tuple<InviteModel, bool>(null, true);
            }

            var createUserDTO = new UserCreateDTO()
            {
                Email = email,
                Step = RegistrationStep.New,
                Status = UserStatus.Member
            };
            user = (await _usersService.CreateUserWithPasswordRestore(createUserDTO, code)).Data;

            var signUpStep = await _signUpRepository.GetRegistrationStepByUserId(user.Id);
            if (signUpStep.Step == RegistrationStep.New)
            {
                isAlreadyRegistered = false;
            }

            var companyUser = await _companiesUsersRepository.GetCompanyUser(companyId, user.Id);
            if (companyUser != null)
            {
                return null;
            }

            await _companiesUsersRepository.AddCompanyUser(companyId, user.Id, UserStatus.Pending);

            var invite = new InviteModel
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

            return new Tuple<InviteModel, bool>(invite, isAlreadyRegistered);
        }
    }
}