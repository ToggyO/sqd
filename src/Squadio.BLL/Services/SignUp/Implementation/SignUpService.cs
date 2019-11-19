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
using Squadio.BLL.Services.Email;
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

        /*
        public async Task<Response> SignUpMemberEmail(SignUpMemberDTO dto)
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

            var inviteResponse = await _invitesProvider.GetInviteByCode(dto.InviteCode);

            if (!inviteResponse.IsSuccess || inviteResponse.Data?.Code != dto.InviteCode ||
                inviteResponse.Data?.Activated == true)
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

            return await SignUpMemberEmail(new SignUpMemberDTO
            {
                Email = infoFromGoogleToken.Email,
                Username = infoFromGoogleToken.Name,
                Password = dto.Password,
                InviteCode = dto.InviteCode
            });
        }
        */

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
                Step = RegistrationStep.New
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

            user = new UserModel
            {
                Name = $"{infoFromGoogleToken.Name}",
                Email = infoFromGoogleToken.Email,
                CreatedDate = DateTime.Now
            };
            
            user = await _usersRepository.Create(user);

            await _repository.SetRegistrationStep(user.Id, RegistrationStep.EmailConfirmed);

            var result = _mapper.Map<UserModel, UserDTO>(user);

            return new Response<UserDTO>
            {
                Data = result
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
                        RegistrationStep = new UserRegistrationStepDTO()
                        {
                            Step = (int) step.Step,
                            StepName = step.Step.ToString()
                        }
                    }
                };
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
                    RegistrationStep = new UserRegistrationStepDTO()
                    {
                        Step = (int) step.Step,
                        StepName = step.Step.ToString()
                    }
                }
            };
        }

        public async Task<Response<SignUpStepDTO<UserDTO>>> SignUpUsername(Guid id, UserUpdateDTO updateDTO)
        {
            var step = await _repository.GetRegistrationStepByUserId(id);

            if (step.Step >= RegistrationStep.UsernameEntered)
            {
                return new BusinessConflictErrorResponse<SignUpStepDTO<UserDTO>>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.InvalidRegistrationStep,
                        Message = ErrorMessages.Business.InvalidRegistrationStep
                    }
                })
                {
                    Data = new SignUpStepDTO<UserDTO>
                    {
                        RegistrationStep = new UserRegistrationStepDTO()
                        {
                            Step = (int) step.Step,
                            StepName = step.Step.ToString()
                        }
                    }
                };
            }

            var userResponse = await _usersService.UpdateUser(id, updateDTO);
            var user = userResponse.Data;

            step = await _repository.SetRegistrationStep(user.Id, RegistrationStep.UsernameEntered);

            return new Response<SignUpStepDTO<UserDTO>>
            {
                Data = new SignUpStepDTO<UserDTO>
                {
                    Data = user,
                    RegistrationStep = new UserRegistrationStepDTO()
                    {
                        Step = (int) step.Step,
                        StepName = step.Step.ToString()
                    }
                }
            };
        }

        public async Task<Response<SignUpStepDTO<CompanyDTO>>> SignUpCompany(Guid userId, CreateCompanyDTO dto)
        {
            var step = await _repository.GetRegistrationStepByUserId(userId);

            if (step.Step >= RegistrationStep.CompanyCreated)
            {
                return new BusinessConflictErrorResponse<SignUpStepDTO<CompanyDTO>>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.InvalidRegistrationStep,
                        Message = ErrorMessages.Business.InvalidRegistrationStep
                    }
                })
                {
                    Data = new SignUpStepDTO<CompanyDTO>
                    {
                        RegistrationStep = new UserRegistrationStepDTO()
                        {
                            Step = (int) step.Step,
                            StepName = step.Step.ToString()
                        }
                    }
                };
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
                    RegistrationStep = new UserRegistrationStepDTO()
                    {
                        Step = (int) step.Step,
                        StepName = step.Step.ToString()
                    }
                }
            };
        }

        public async Task<Response<SignUpStepDTO<TeamDTO>>> SignUpTeam(Guid userId, TeamCreateDTO dto)
        {
            var step = await _repository.GetRegistrationStepByUserId(userId);

            if (step.Step >= RegistrationStep.TeamCreated)
            {
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
                        RegistrationStep = new UserRegistrationStepDTO()
                        {
                            Step = (int) step.Step,
                            StepName = step.Step.ToString()
                        }
                    }
                };
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
                        RegistrationStep = new UserRegistrationStepDTO()
                        {
                            Step = (int) step.Step,
                            StepName = step.Step.ToString()
                        }
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
                    RegistrationStep = new UserRegistrationStepDTO()
                    {
                        Step = (int) step.Step,
                        StepName = step.Step.ToString()
                    }
                }
            };
        }

        public async Task<Response<SignUpStepDTO<ProjectDTO>>> SignUpProject(Guid userId, CreateProjectDTO dto)
        {
            var step = await _repository.GetRegistrationStepByUserId(userId);

            if (step.Step >= RegistrationStep.ProjectCreated)
            {
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
                        RegistrationStep = new UserRegistrationStepDTO()
                        {
                            Step = (int) step.Step,
                            StepName = step.Step.ToString()
                        }
                    }
                };
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
                        RegistrationStep = new UserRegistrationStepDTO()
                        {
                            Step = (int) step.Step,
                            StepName = step.Step.ToString()
                        }
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
                    RegistrationStep = new UserRegistrationStepDTO()
                    {
                        Step = (int) step.Step,
                        StepName = step.Step.ToString()
                    },
                    Data = project.Data
                }
            };
        }

        public async Task<Response<SignUpStepDTO>> SignUpDone(Guid userId)
        {
            var step = await _repository.GetRegistrationStepByUserId(userId);

            if (step.Step >= RegistrationStep.Done)
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
                        RegistrationStep = new UserRegistrationStepDTO()
                        {
                            Step = (int) step.Step,
                            StepName = step.Step.ToString()
                        }
                    }
                };
            }

            step = await _repository.SetRegistrationStep(userId, RegistrationStep.Done);
            
            return new Response<SignUpStepDTO>
            {
                Data = new SignUpStepDTO
                {
                    RegistrationStep = new UserRegistrationStepDTO()
                    {
                        Step = (int) step.Step,
                        StepName = step.Step.ToString()
                    }
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
                        RegistrationStep = new UserRegistrationStepDTO()
                        {
                            Step = (int) step.Step,
                            StepName = step.Step.ToString()
                        }
                    }
                };
            }
            
            await _confirmEmailService.AddRequest(user.Id, user.Email);
            
            return new Response<SignUpStepDTO>
            {
                Data = new SignUpStepDTO
                {
                    RegistrationStep = new UserRegistrationStepDTO()
                    {
                        Step = (int) step.Step,
                        StepName = step.Step.ToString()
                    }
                }
            };
        }
    }
}