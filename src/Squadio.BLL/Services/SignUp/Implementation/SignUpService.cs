using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Mapper;
using Microsoft.Extensions.Options;
using Squadio.BLL.Providers.Invites;
using Squadio.BLL.Services.Companies;
using Squadio.BLL.Services.Email;
using Squadio.BLL.Services.Invites;
using Squadio.BLL.Services.Projects;
using Squadio.BLL.Services.Teams;
using Squadio.BLL.Services.Users;
using Squadio.Common.Models.Email;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;
using Squadio.Common.Settings;
using Squadio.DAL.Repository.SignUp;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Companies;
using Squadio.DTO.Projects;
using Squadio.DTO.SignUp;
using Squadio.DTO.Teams;
using Squadio.DTO.Users;

namespace Squadio.BLL.Services.SignUp.Implementation
{
    public class SignUpService : ISignUpService
    {
        private readonly ISignUpRepository _repository;
        private readonly IUsersRepository _usersRepository;
        private readonly IEmailService<PasswordSetEmailModel> _passwordSetMailService;
        private readonly IOptions<GoogleSettings> _googleSettings;
        private readonly IInvitesProvider _invitesProvider;
        private readonly IInvitesService _invitesService;
        private readonly IUsersService _usersService;
        private readonly ICompaniesService _companiesService;
        private readonly ITeamsService _teamsService;
        private readonly IProjectsService _projectsService;
        private readonly IMapper _mapper;

        public SignUpService(ISignUpRepository repository
            , IUsersRepository usersRepository
            , IEmailService<PasswordSetEmailModel> passwordSetMailService
            , IOptions<GoogleSettings> googleSettings
            , IInvitesProvider invitesProvider
            , IInvitesService invitesService
            , IUsersService usersService
            , ICompaniesService companiesService
            , ITeamsService teamsService
            , IProjectsService projectsService
            , IMapper mapper
        )
        {
            _repository = repository;
            _usersRepository = usersRepository;
            _passwordSetMailService = passwordSetMailService;
            _googleSettings = googleSettings;
            _invitesProvider = invitesProvider;
            _invitesService = invitesService;
            _usersService = usersService;
            _companiesService = companiesService;
            _teamsService = teamsService;
            _projectsService = projectsService;
            _mapper = mapper;
        }

        public async Task<Response<UserDTO>> SignUpMemberEmail(SignUpMemberDTO dto)
        {
            var user = await _usersRepository.GetByEmail(dto.Email);
            if (user != null)
            {
                return new ErrorResponse<UserDTO>
                {
                    Code = ErrorCodes.Business.EmailExists,
                    Message = ErrorMessages.Business.EmailExists,
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Errors = new List<Error>
                    {
                        new Error
                        {
                            Code = ErrorCodes.Business.EmailExists,
                            Message = ErrorMessages.Business.EmailExists,
                            Field = ErrorFields.User.Email
                        }
                    }
                };
            }

            var inviteResponse = await _invitesProvider.GetInviteByEmail(dto.Email);

            if (!inviteResponse.IsSuccess || inviteResponse.Data?.Code != dto.InviteCode ||
                inviteResponse.Data?.Activated == true)
            {
                return new ErrorResponse<UserDTO>
                {
                    Code = ErrorCodes.Security.InviteInvalid,
                    Message = ErrorMessages.Security.InviteInvalid,
                    HttpStatusCode = HttpStatusCode.BadRequest
                };
            }

            user = new UserModel
            {
                Name = dto.Username,
                Email = dto.Email,
                CreatedDate = DateTime.UtcNow
            };

            user = await _usersRepository.Create(user);

            var setPasswordResponse = await _usersService.SetPassword(dto.Email, dto.Password);
            var result = setPasswordResponse.Data;

            await _repository.SetRegistrationStep(user.Id, RegistrationStep.Done);

            return new Response<UserDTO>
            {
                Data = result
            };
        }

        public async Task<Response<UserDTO>> SignUpMemberGoogle(SignUpMemberGoogleDTO dto)
        {
            var infoFromGoogleToken = await GoogleJsonWebSignature.ValidateAsync(dto.Token);

            if ((string) infoFromGoogleToken.Audience != _googleSettings.Value.ClientId)
            {
                return new ErrorResponse<UserDTO>
                {
                    Code = ErrorCodes.Security.GoogleAccessTokenInvalid,
                    Message = ErrorMessages.Security.GoogleAccessTokenInvalid,
                    HttpStatusCode = HttpStatusCode.BadRequest
                };
            }

            return await SignUpMemberEmail(new SignUpMemberDTO
            {
                Email = infoFromGoogleToken.Email,
                Username = infoFromGoogleToken.Name,
                Password = dto.Password,
                InviteCode = dto.InviteCode
            });
        }

        public async Task<Response> SignUp(string email)
        {
            var user = await _usersRepository.GetByEmail(email);
            if (user != null)
            {
                return new ErrorResponse<UserDTO>
                {
                    Code = ErrorCodes.Business.EmailExists,
                    Message = ErrorMessages.Business.EmailExists,
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Errors = new List<Error>
                    {
                        new Error
                        {
                            Code = ErrorCodes.Business.EmailExists,
                            Message = ErrorMessages.Business.EmailExists,
                            Field = ErrorFields.User.Email
                        }
                    }
                };
            }

            var code = _usersService.GenerateCode();

            await _passwordSetMailService.Send(new PasswordSetEmailModel
            {
                Code = code,
                To = email
            });

            user = new UserModel
            {
                Email = email,
                CreatedDate = DateTime.UtcNow
            };

            user = await _usersRepository.Create(user);

            await _usersRepository.AddChangePasswordRequest(user.Id, code);

            await _repository.SetRegistrationStep(user.Id, RegistrationStep.New);

            return new Response();
        }

        public async Task<Response<UserDTO>> SignUpGoogle(string googleToken)
        {
            var infoFromGoogleToken = await GoogleJsonWebSignature.ValidateAsync(googleToken);

            if ((string) infoFromGoogleToken.Audience != _googleSettings.Value.ClientId)
            {
                return new ErrorResponse<UserDTO>
                {
                    Code = ErrorCodes.Security.GoogleAccessTokenInvalid,
                    Message = ErrorMessages.Security.GoogleAccessTokenInvalid,
                    HttpStatusCode = HttpStatusCode.BadRequest
                };
            }

            var user = await _usersRepository.GetByEmail(infoFromGoogleToken.Email);
            if (user != null)
            {
                return new ErrorResponse<UserDTO>
                {
                    Code = ErrorCodes.Business.EmailExists,
                    Message = ErrorMessages.Business.EmailExists,
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Errors = new List<Error>
                    {
                        new Error
                        {
                            Code = ErrorCodes.Business.EmailExists,
                            Message = ErrorMessages.Business.EmailExists,
                            Field = ErrorFields.User.Email
                        }
                    }
                };
            }

            user = new UserModel
            {
                Name = $"{infoFromGoogleToken.Name}",
                Email = infoFromGoogleToken.Email,
                CreatedDate = DateTime.Now
            };
            user = await _usersRepository.Create(user);

            await _repository.SetRegistrationStep(user.Id, RegistrationStep.EmailConfirmed);

            var code = _usersService.GenerateCode();
            await _passwordSetMailService.Send(new PasswordSetEmailModel
            {
                Code = code,
                To = user.Email
            });
            await _usersRepository.AddChangePasswordRequest(user.Id, code);

            var result = _mapper.Map<UserModel, UserDTO>(user);

            return new Response<UserDTO>
            {
                Data = result
            };
        }

        public async Task<Response<UserDTO>> SignUpPassword(string email, string code, string password)
        {
            var step = await _repository.GetRegistrationStepByEmail(email);

            if (step.Step >= RegistrationStep.PasswordEntered)
            {
                return new ErrorResponse<UserDTO>
                {
                    Code = ErrorCodes.Business.InvalidRegistrationStep,
                    Message = ErrorMessages.Business.InvalidRegistrationStep,
                    // TODO: find correct http code for this
                    HttpStatusCode = HttpStatusCode.Conflict
                };
            }

            var userResponse = await _usersService.SetPassword(email, code, password);
            var user = userResponse.Data;

            await _repository.SetRegistrationStep(user.Id, RegistrationStep.PasswordEntered);

            return new Response<UserDTO>
            {
                Data = user
            };
        }

        public async Task<Response<UserDTO>> SignUpUsername(Guid id, UserUpdateDTO updateDTO)
        {
            var step = await _repository.GetRegistrationStepByUserId(id);

            if (step.Step >= RegistrationStep.UsernameEntered)
            {
                return new ErrorResponse<UserDTO>
                {
                    Code = ErrorCodes.Business.InvalidRegistrationStep,
                    Message = ErrorMessages.Business.InvalidRegistrationStep,
                    // TODO: find correct http code for this
                    HttpStatusCode = HttpStatusCode.Conflict
                };
            }

            var userResponse = await _usersService.UpdateUser(id, updateDTO);
            var user = userResponse.Data;

            await _repository.SetRegistrationStep(user.Id, RegistrationStep.UsernameEntered);

            return new Response<UserDTO>
            {
                Data = user
            };
        }

        public async Task<Response<CompanyDTO>> SignUpCompany(Guid userId, CreateCompanyDTO dto)
        {
            var step = await _repository.GetRegistrationStepByUserId(userId);

            if (step.Step >= RegistrationStep.CompanyCreated)
            {
                return new ErrorResponse<CompanyDTO>
                {
                    Code = ErrorCodes.Business.InvalidRegistrationStep,
                    Message = ErrorMessages.Business.InvalidRegistrationStep,
                    // TODO: find correct http code for this
                    HttpStatusCode = HttpStatusCode.Conflict
                };
            }

            var company = await _companiesService.Create(userId, dto);

            await _repository.SetRegistrationStep(userId, RegistrationStep.CompanyCreated);

            return company;
        }

        public async Task<Response<TeamDTO>> SignUpTeam(Guid userId, CreateTeamDTO dto)
        {
            var step = await _repository.GetRegistrationStepByUserId(userId);

            if (step.Step >= RegistrationStep.TeamCreated)
            {
                return new ErrorResponse<TeamDTO>
                {
                    Code = ErrorCodes.Business.InvalidRegistrationStep,
                    Message = ErrorMessages.Business.InvalidRegistrationStep,
                    // TODO: find correct http code for this
                    HttpStatusCode = HttpStatusCode.Conflict
                };
            }

            var team = await _teamsService.Create(userId, dto);

            await _repository.SetRegistrationStep(userId, RegistrationStep.TeamCreated);

            try
            {
                if (dto.Emails?.Length > 0)
                {
                    foreach (var email in dto.Emails)
                    {
                        var res = await _invitesService.InviteToTeam(step.User.Name
                            , team.Data.Name
                            , team.Data.Id
                            , email);
                    }
                }
            }
            catch (Exception e)
            {
                // TODO: make it correct
                return new ErrorResponse<TeamDTO>
                {
                    Message = "Not all emails sent"
                };
            }

            return team;
        }

        public async Task<Response<ProjectDTO>> SignUpProject(Guid userId, CreateProjectDTO dto)
        {
            var step = await _repository.GetRegistrationStepByUserId(userId);

            if (step.Step >= RegistrationStep.ProjectCreated)
            {
                return new ErrorResponse<ProjectDTO>
                {
                    Code = ErrorCodes.Business.InvalidRegistrationStep,
                    Message = ErrorMessages.Business.InvalidRegistrationStep,
                    // TODO: find correct http code for this
                    HttpStatusCode = HttpStatusCode.Conflict
                };
            }

            var project = await _projectsService.Create(userId, dto);

            await _repository.SetRegistrationStep(userId, RegistrationStep.ProjectCreated);

            return project;
        }

        public async Task<Response> SignUpDone(Guid userId)
        {
            var step = await _repository.GetRegistrationStepByUserId(userId);

            if (step.Step >= RegistrationStep.Done)
            {
                return new ErrorResponse
                {
                    Code = ErrorCodes.Business.InvalidRegistrationStep,
                    Message = ErrorMessages.Business.InvalidRegistrationStep,
                    // TODO: find correct http code for this
                    HttpStatusCode = HttpStatusCode.Conflict
                };
            }

            await _repository.SetRegistrationStep(userId, RegistrationStep.Done);
            return new Response();
        }
    }
}