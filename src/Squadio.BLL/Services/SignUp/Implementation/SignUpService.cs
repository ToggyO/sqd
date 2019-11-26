using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Mapper;
using Microsoft.Extensions.Options;
using Squadio.BLL.Providers.Companies;
using Squadio.BLL.Providers.Invites;
using Squadio.BLL.Providers.Teams;
using Squadio.BLL.Services.Companies;
using Squadio.BLL.Services.ConfirmEmail;
using Squadio.BLL.Services.Projects;
using Squadio.BLL.Services.Teams;
using Squadio.BLL.Services.Users;
using Squadio.Common.Models.Email;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.Common.Settings;
using Squadio.DAL.Repository.ConfirmEmail;
using Squadio.DAL.Repository.SignUp;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Projects;
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
        //private readonly IOptions<GoogleSettings> _googleSettings;
        private readonly IInvitesProvider _invitesProvider;
        private readonly IUsersService _usersService;
        private readonly ICompaniesService _companiesService;
        private readonly ICompaniesProvider _companiesProvider;
        private readonly ITeamsService _teamsService;
        private readonly ITeamsProvider _teamsProvider;
        private readonly IProjectsService _projectsService;
        private readonly IConfirmEmailService _confirmEmailService;
        private readonly IMapper _mapper;

        public SignUpService(ISignUpRepository repository
            , IUsersRepository usersRepository
            //, IOptions<GoogleSettings> googleSettings
            , IInvitesProvider invitesProvider
            , IUsersService usersService
            , ICompaniesService companiesService
            , ICompaniesProvider companiesProvider
            , ITeamsService teamsService
            , ITeamsProvider teamsProvider
            , IProjectsService projectsService
            , IConfirmEmailService confirmEmailService
            , IMapper mapper
        )
        {
            _repository = repository;
            _usersRepository = usersRepository;
            //_googleSettings = googleSettings;
            _invitesProvider = invitesProvider;
            _usersService = usersService;
            _companiesService = companiesService;
            _companiesProvider = companiesProvider;
            _teamsService = teamsService;
            _teamsProvider = teamsProvider;
            _projectsService = projectsService;
            _confirmEmailService = confirmEmailService;
            _mapper = mapper;
        }

        public async Task<Response<UserDTO>> SignUpMemberEmail(SignUpMemberDTO dto)
        {
            var inviteResponse = await _invitesProvider.GetInviteByCode(dto.InviteCode);

            if (!inviteResponse.IsSuccess 
                || inviteResponse.Data?.Code != dto.InviteCode 
                || inviteResponse.Data?.Activated == true)
            {
                return new SecurityErrorResponse<UserDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.InviteInvalid,
                        Message = ErrorMessages.Security.InviteInvalid
                    }
                });
            }

            var invite = inviteResponse.Data;
            
            var user = await _usersRepository.GetByEmail(invite.Email);
            var step = await _repository.GetRegistrationStepByUserId(user.Id);
            var stepValidate = ValidateStep(step, RegistrationStep.EmailConfirmed);
            if (!stepValidate.IsSuccess)
            {
                var errorResponse = (ErrorResponse<SignUpStepDTO>) stepValidate;
                
                return new ErrorResponse<UserDTO>
                {
                    Message = errorResponse.Message,
                    Code = errorResponse.Code,
                    Errors = errorResponse.Errors,
                    HttpStatusCode = errorResponse.HttpStatusCode
                };
            }

            var resetPasswordResponse = await _usersService.ResetPassword(dto.InviteCode, dto.Password);

            if (!resetPasswordResponse.IsSuccess)
            {
                return resetPasswordResponse;
            }

            await _repository.SetRegistrationStep(user.Id, RegistrationStep.EmailConfirmed);

            return new Response<UserDTO>
            {
                Data = _mapper.Map<UserModel, UserDTO>(user)
            };
        }

        public async Task<Response> SignUpMemberGoogle(SignUpMemberGoogleDTO dto)
        {
            GoogleJsonWebSignature.Payload infoFromGoogleToken;

            try
            {
                infoFromGoogleToken = await GoogleJsonWebSignature.ValidateAsync(dto.Token);
            }
            catch
            {
                return new ForbiddenErrorResponse<UserDTO>(new[]
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.GoogleTokenInvalid,
                        Message = ErrorMessages.Security.GoogleTokenInvalid,
                        Field = ErrorFields.User.GoogleToken
                    }
                });
            }
            
            var inviteResponse = await _invitesProvider.GetInviteByCode(dto.InviteCode);

            if (!inviteResponse.IsSuccess 
                || inviteResponse.Data?.Code != dto.InviteCode 
                || inviteResponse.Data?.Activated == true)
            {
                return new SecurityErrorResponse<UserDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.InviteInvalid,
                        Message = ErrorMessages.Security.InviteInvalid
                    }
                });
            }

            var invite = inviteResponse.Data;
            
            var user = await _usersRepository.GetByEmail(invite?.Email);
            var step = await _repository.GetRegistrationStepByUserId(user.Id);
            var stepValidate = ValidateStep(step, RegistrationStep.EmailConfirmed);
            if (!stepValidate.IsSuccess)
            {
                return stepValidate;
            }

            user.Name = infoFromGoogleToken.Name;
            
            step = await _repository.SetRegistrationStep(user.Id, RegistrationStep.ProjectCreated);
            
            return new Response();
        }

        public async Task<Response<SignUpStepDTO<UserDTO>>> SignUpMemberUsername(Guid userId, UserUpdateDTO updateDTO)
        {
            var step = await _repository.GetRegistrationStepByUserId(userId);
            var stepValidate = ValidateStep<UserDTO>(step, RegistrationStep.ProjectCreated);
            if (!stepValidate.IsSuccess)
            {
                return stepValidate;
            }

            var user = await _usersRepository.GetById(userId);
            user.Name = updateDTO.Name;
            user = await _usersRepository.Update(user);

            step = await _repository.SetRegistrationStep(user.Id, RegistrationStep.ProjectCreated);
            
            return new Response<SignUpStepDTO<UserDTO>>
            {
                Data = new SignUpStepDTO<UserDTO>
                {
                    Data = _mapper.Map<UserModel, UserDTO>(user),
                    RegistrationStep = _mapper.Map<UserRegistrationStepModel, UserRegistrationStepDTO>(step)
                }
            };
        }

        public async Task<Response> SignUp(string email, string password)
        {
            var user = await _usersRepository.GetByEmail(email);
            if (user != null)
            {
                return new BusinessConflictErrorResponse<UserDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.EmailExists,
                        Message = ErrorMessages.Business.EmailExists,
                        Field = ErrorFields.User.Email
                    }
                });
            }
            
            var createUserDTO = new UserCreateDTO()
            {
                Email = email,
                Step = RegistrationStep.New,
                Status = UserStatus.Admin
            };

            var createResponse = await _usersService.CreateUser(createUserDTO);

            if (!createResponse.IsSuccess)
                return createResponse;

            var createdUser = createResponse.Data;
            
            await _usersService.SetPassword(email, password);

            await _confirmEmailService.AddRequest(createdUser.Id, createdUser.Email);

            return new Response<UserDTO>
            {
                Data = createdUser
            };
        }

        public async Task<Response> SignUpGoogle(string googleToken)
        {
            GoogleJsonWebSignature.Payload infoFromGoogleToken;

            try
            {
                infoFromGoogleToken = await GoogleJsonWebSignature.ValidateAsync(googleToken);
            }
            catch
            {
                return new ForbiddenErrorResponse<UserDTO>(new[]
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.GoogleTokenInvalid,
                        Message = ErrorMessages.Security.GoogleTokenInvalid,
                        Field = ErrorFields.User.GoogleToken
                    }
                });
            }

            //TODO: think how validate google token
            /*
            if ((string) infoFromGoogleToken.Audience != _googleSettings.Value.ClientId)
            {
                return new ForbiddenErrorResponse<UserDTO>(new[]
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.GoogleTokenInvalid,
                        Message = ErrorMessages.Security.GoogleTokenInvalid,
                        Field = ErrorFields.User.GoogleToken
                    }
                });
            }
            */

            var user = await _usersRepository.GetByEmail(infoFromGoogleToken.Email);
            if (user != null)
            {
                return new BusinessConflictErrorResponse<UserDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.EmailExists,
                        Message = ErrorMessages.Business.EmailExists,
                        Field = ErrorFields.User.GoogleToken
                    }
                });
            }
            
            var createUserDTO = new UserCreateDTO()
            {
                Email = infoFromGoogleToken.Email,
                Name = infoFromGoogleToken.Name,
                Step = RegistrationStep.EmailConfirmed,
                Status = UserStatus.Admin
            };

            var createResponse = await _usersService.CreateUser(createUserDTO);

            if (!createResponse.IsSuccess)
                return createResponse;

            var createdUser = createResponse.Data;

            return new Response<UserDTO>
            {
                Data = createdUser
            };
        }
        
        public async Task<Response<SignUpStepDTO>> SendNewCode(string email)
        {
            var user = await _usersRepository.GetByEmail(email);
            return await SendNewCode(user);
        }

        public async Task<Response<SignUpStepDTO>> SendNewCode(Guid userId)
        {
            var user = await _usersRepository.GetById(userId);
            return await SendNewCode(user);
        }

        public async Task<Response<SignUpStepDTO>> SignUpConfirm(Guid userId, string code)
        {
            var step = await _repository.GetRegistrationStepByUserId(userId);
            var stepValidate = ValidateStep(step, RegistrationStep.EmailConfirmed);
            if (!stepValidate.IsSuccess)
            {
                return stepValidate;
            }

            var request = await _confirmEmailService.GetRequest(userId, code);

            if (!request.IsSuccess)
            {
                var errorResponse = (ErrorResponse<UserConfirmEmailRequestDTO>) request;
                
                return new ErrorResponse<SignUpStepDTO>
                {
                    Message = errorResponse.Message,
                    Code = errorResponse.Code,
                    Errors = errorResponse.Errors,
                    HttpStatusCode = errorResponse.HttpStatusCode
                };
            }

            if (request.Data == null || request.Data?.IsActivated == true)
            {
                return new ForbiddenErrorResponse<SignUpStepDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.ConfirmationCodeInvalid,
                        Message = ErrorMessages.Security.ConfirmationCodeInvalid
                    }
                });
            }
            
            // TODO: Check lifetime of request if needed

            await _confirmEmailService.ActivateAllRequests(userId);

            step = await _repository.SetRegistrationStep(userId, RegistrationStep.EmailConfirmed);
            
            return new Response<SignUpStepDTO>
            {
                Data = new SignUpStepDTO
                {
                    RegistrationStep = _mapper.Map<UserRegistrationStepModel, UserRegistrationStepDTO>(step)
                }
            };
        }

        public async Task<Response<SignUpStepDTO<UserDTO>>> SignUpUsername(Guid id, UserUpdateDTO updateDTO)
        {
            var step = await _repository.GetRegistrationStepByUserId(id);
            var stepValidate = ValidateStep<UserDTO>(step, RegistrationStep.UsernameEntered);
            if (!stepValidate.IsSuccess)
            {
                return stepValidate;
            }

            var userResponse = await _usersService.UpdateUser(id, updateDTO);
            var user = userResponse.Data;

            step = await _repository.SetRegistrationStep(user.Id, RegistrationStep.UsernameEntered);

            return new Response<SignUpStepDTO<UserDTO>>
            {
                Data = new SignUpStepDTO<UserDTO>
                {
                    Data = user,
                    RegistrationStep = _mapper.Map<UserRegistrationStepModel, UserRegistrationStepDTO>(step)
                }
            };
        }

        public async Task<Response<SignUpStepDTO<CompanyDTO>>> SignUpCompany(Guid userId, CreateCompanyDTO dto)
        {
            var step = await _repository.GetRegistrationStepByUserId(userId);
            var stepValidate = ValidateStep<CompanyDTO>(step, RegistrationStep.CompanyCreated);
            if (!stepValidate.IsSuccess)
            {
                return stepValidate;
            }

            var company = await _companiesService.Create(userId, dto);

            if (company.IsSuccess)
            {
                step = await _repository.SetRegistrationStep(userId, RegistrationStep.CompanyCreated);
            }

            
            return new Response<SignUpStepDTO<CompanyDTO>>
            {
                Data = new SignUpStepDTO<CompanyDTO>
                {
                    Data = company.Data,
                    RegistrationStep = _mapper.Map<UserRegistrationStepModel, UserRegistrationStepDTO>(step)
                }
            };
        }

        public async Task<Response<SignUpStepDTO<TeamDTO>>> SignUpTeam(Guid userId, TeamCreateDTO dto)
        {
            var step = await _repository.GetRegistrationStepByUserId(userId);
            var stepValidate = ValidateStep<TeamDTO>(step, RegistrationStep.TeamCreated);
            if (!stepValidate.IsSuccess)
            {
                return stepValidate;
            }

            var companyPage = await _companiesProvider.GetUserCompanies(userId, new PageModel());
            var company = companyPage.Data.Items.FirstOrDefault();

            if (company == null)
            {
                step = await _repository.SetRegistrationStep(userId, RegistrationStep.UsernameEntered);
                
                return new BusinessConflictErrorResponse<SignUpStepDTO<TeamDTO>>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.InvalidRegistrationStep,
                        Message = ErrorMessages.Business.InvalidRegistrationStep
                    }
                })
                {
                    Data = new SignUpStepDTO<TeamDTO>
                    {
                        RegistrationStep = _mapper.Map<UserRegistrationStepModel, UserRegistrationStepDTO>(step)
                    }
                };
            }

            var team = await _teamsService.Create(userId, company.CompanyId, dto);

            if (team.IsSuccess)
            {
                step = await _repository.SetRegistrationStep(userId, RegistrationStep.TeamCreated);
            }

            
            return new Response<SignUpStepDTO<TeamDTO>>
            {
                Data = new SignUpStepDTO<TeamDTO>
                {
                    Data = team.Data,
                    RegistrationStep = _mapper.Map<UserRegistrationStepModel, UserRegistrationStepDTO>(step)
                }
            };
        }

        public async Task<Response<SignUpStepDTO<ProjectDTO>>> SignUpProject(Guid userId, CreateProjectDTO dto)
        {
            var step = await _repository.GetRegistrationStepByUserId(userId);
            var stepValidate = ValidateStep<ProjectDTO>(step, RegistrationStep.ProjectCreated);
            if (!stepValidate.IsSuccess)
            {
                return stepValidate;
            }

            var teamPage = await _teamsProvider.GetUserTeams(userId, new PageModel());
            var team = teamPage.Data.Items.FirstOrDefault();

            if (team == null)
            {
                step = await _repository.SetRegistrationStep(userId, RegistrationStep.UsernameEntered);
                
                return new BusinessConflictErrorResponse<SignUpStepDTO<ProjectDTO>>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.InvalidRegistrationStep,
                        Message = ErrorMessages.Business.InvalidRegistrationStep
                    }
                })
                {
                    Data = new SignUpStepDTO<ProjectDTO>
                    {
                        RegistrationStep = _mapper.Map<UserRegistrationStepModel, UserRegistrationStepDTO>(step)
                    }
                };
            }

            var project = await _projectsService.Create(userId, team.TeamId, dto);

            if (project.IsSuccess)
            {
                step = await _repository.SetRegistrationStep(userId, RegistrationStep.ProjectCreated);
            }
            
            return new Response<SignUpStepDTO<ProjectDTO>>
            {
                Data = new SignUpStepDTO<ProjectDTO>
                {
                    RegistrationStep = _mapper.Map<UserRegistrationStepModel, UserRegistrationStepDTO>(step),
                    Data = project.Data
                }
            };
        }

        public async Task<Response<SignUpStepDTO>> SignUpDone(Guid userId)
        {
            var step = await _repository.GetRegistrationStepByUserId(userId);
            var stepValidate = ValidateStep(step, RegistrationStep.Done);
            if (!stepValidate.IsSuccess)
            {
                return stepValidate;
            }

            step = await _repository.SetRegistrationStep(userId, RegistrationStep.Done);
            
            return new Response<SignUpStepDTO>
            {
                Data = new SignUpStepDTO
                {
                    RegistrationStep = _mapper.Map<UserRegistrationStepModel, UserRegistrationStepDTO>(step)
                }
            };
        }

        private async Task<Response<SignUpStepDTO>> SendNewCode(UserModel user)
        {
            if (user == null)
            {
                return new BusinessConflictErrorResponse<SignUpStepDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.UserDoesNotExists,
                        Message = ErrorMessages.Business.UserDoesNotExists,
                        Field = ErrorFields.User.Email
                    }
                });
            }
            
            var step = await _repository.GetRegistrationStepByUserId(user.Id);

            if (step.Step >= RegistrationStep.EmailConfirmed)
            {
                return new BusinessConflictErrorResponse<SignUpStepDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.InvalidRegistrationStep,
                        Message = ErrorMessages.Business.InvalidRegistrationStep
                    }
                })
                {
                    Data = new SignUpStepDTO
                    {
                        RegistrationStep = _mapper.Map<UserRegistrationStepModel, UserRegistrationStepDTO>(step)
                    }
                };
            }
            
            await _confirmEmailService.AddRequest(user.Id, user.Email);
            
            return new Response<SignUpStepDTO>
            {
                Data = new SignUpStepDTO
                {
                    RegistrationStep = _mapper.Map<UserRegistrationStepModel, UserRegistrationStepDTO>(step)
                }
            };
        }

        private Response<SignUpStepDTO<T>> ValidateStep<T>(UserRegistrationStepModel model, RegistrationStep step)
        {
            if (model.Step >= step)
            {
                return new BusinessConflictErrorResponse<SignUpStepDTO<T>>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.InvalidRegistrationStep,
                        Message = ErrorMessages.Business.InvalidRegistrationStep
                    }
                })
                {
                    Data = new SignUpStepDTO<T>
                    {
                        RegistrationStep = _mapper.Map<UserRegistrationStepModel, UserRegistrationStepDTO>(model)
                    }
                };
            }
            
            return new Response<SignUpStepDTO<T>>();
        }

        private Response<SignUpStepDTO> ValidateStep(UserRegistrationStepModel model, RegistrationStep step)
        {
            if (model.Step >= step)
            {
                return new BusinessConflictErrorResponse<SignUpStepDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.InvalidRegistrationStep,
                        Message = ErrorMessages.Business.InvalidRegistrationStep
                    }
                })
                {
                    Data = new SignUpStepDTO
                    {
                        RegistrationStep = _mapper.Map<UserRegistrationStepModel, UserRegistrationStepDTO>(model)
                    }
                };
            }
            
            return new Response<SignUpStepDTO>();
        }
    }
}