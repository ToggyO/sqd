using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Google.Apis.Auth;
using Microsoft.Extensions.Logging;
using Squadio.BLL.Providers.Companies;
using Squadio.BLL.Providers.Projects;
using Squadio.BLL.Providers.Teams;
using Squadio.BLL.Providers.Users;
using Squadio.BLL.Services.Companies;
using Squadio.BLL.Services.ConfirmEmail;
using Squadio.BLL.Services.Invites;
using Squadio.BLL.Services.Projects;
using Squadio.BLL.Services.Teams;
using Squadio.BLL.Services.Users;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Invites;
using Squadio.DAL.Repository.SignUp;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Invites;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Models.Companies;
using Squadio.DTO.Models.Projects;
using Squadio.DTO.Models.SignUp;
using Squadio.DTO.Models.Teams;
using Squadio.DTO.Models.Users;
using Squadio.DTO.Models.Users.Settings;
using Squadio.DTO.Projects;

namespace Squadio.BLL.Services.SignUp.Implementations
{
    public class SignUpService : ISignUpService
    {
        private readonly ISignUpRepository _repository;
        private readonly IUsersRepository _usersRepository;
        private readonly IInvitesRepository _invitesRepository;
        private readonly IUsersService _usersService;
        private readonly ICompaniesService _companiesService;
        private readonly ICompaniesProvider _companiesProvider;
        private readonly ITeamsService _teamsService;
        private readonly ITeamsProvider _teamsProvider;
        private readonly IProjectsService _projectsService;
        private readonly IProjectsProvider _projectsProvider;
        private readonly IConfirmEmailService _confirmEmailService;
        private readonly IInvitesService _invitesService;
        private readonly ILogger<SignUpService> _logger;
        private readonly IMapper _mapper;

        public SignUpService(ISignUpRepository repository
            , IUsersRepository usersRepository
            , IInvitesRepository invitesRepository
            , IUsersService usersService
            , ICompaniesService companiesService
            , ICompaniesProvider companiesProvider
            , ITeamsService teamsService
            , ITeamsProvider teamsProvider
            , IProjectsService projectsService
            , IProjectsProvider projectsProvider
            , IConfirmEmailService confirmEmailService
            , IInvitesService invitesService
            , ILogger<SignUpService> logger
            , IMapper mapper)
        {
            _repository = repository;
            _usersRepository = usersRepository;
            _invitesRepository = invitesRepository;
            _usersService = usersService;
            _companiesService = companiesService;
            _companiesProvider = companiesProvider;
            _teamsService = teamsService;
            _teamsProvider = teamsProvider;
            _projectsService = projectsService;
            _projectsProvider = projectsProvider;
            _confirmEmailService = confirmEmailService;
            _invitesService = invitesService;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<Response> SimpleSignUp(SignUpSimpleDTO dto)
        {
            var createDto = _mapper.Map<UserCreateDTO>(dto);
            await _usersService.CreateUser(createDto);
            await _usersService.SetPassword(dto.Email, dto.Password);
            return new Response();
        }
        
        public async Task<Response<UserDTO>> SignUpMemberEmail(SignUpMemberDTO dto)
        {
            var inviteResponse = await GetInviteByCode(dto.InviteCode);

            if (!inviteResponse.IsSuccess 
                || inviteResponse.Data?.Code != dto.InviteCode 
                || inviteResponse.Data?.IsDeleted == true)
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

            var resetPasswordResponse = await _usersService.ResetPasswordConfirm(dto.InviteCode, dto.Password);

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
            
            var inviteResponse = await GetInviteByCode(dto.InviteCode);

            if (!inviteResponse.IsSuccess 
                || inviteResponse.Data?.Code != dto.InviteCode 
                || inviteResponse.Data?.IsDeleted == true)
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
                MembershipStatus = MembershipStatus.SuperAdmin,
                SignUpBy = SignUpType.Email
            };

            var createResponse = await _usersService.CreateUser(createUserDTO);

            await _repository.SetRegistrationStep(createResponse.Data.Id, createUserDTO.Step, createUserDTO.MembershipStatus);

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
                return new PermissionDeniedErrorResponse<UserDTO>(new[]
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
                MembershipStatus = MembershipStatus.SuperAdmin,
                SignUpBy = SignUpType.Google
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

            if (request.Data == null || request.Data?.IsDeleted == true)
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

            switch (step.Status)
            {
                case MembershipStatus.Admin:
                    return await SignUpUsernameAdmin(id, updateDTO);
                case MembershipStatus.Member:
                    return await SignUpUsernameMember(id, updateDTO);
                default:
                    return new BusinessConflictErrorResponse<SignUpStepDTO<UserDTO>>(new Error
                    {
                        Code = ErrorCodes.Global.BusinessConflict,
                        Message = ErrorMessages.Global.BusinessConflict
                    });
            }
        }

        public async Task<Response<SignUpStepDTO<CompanyDTO>>> SignUpCompany(Guid userId, CompanyCreateDTO dto)
        {
            var step = await _repository.GetRegistrationStepByUserId(userId);
            var stepValidate = ValidateStep<CompanyDTO>(step, RegistrationStep.CompanyCreated);
            if (!stepValidate.IsSuccess)
            {
                return stepValidate;
            }

            if (step.Status != MembershipStatus.Admin)
            {
                return new ForbiddenErrorResponse<SignUpStepDTO<CompanyDTO>>(new Error
                {
                    Code = ErrorCodes.Security.Forbidden,
                    Message = ErrorMessages.Security.Forbidden
                });
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

            if (step.Status != MembershipStatus.Admin)
            {
                return new ForbiddenErrorResponse<SignUpStepDTO<TeamDTO>>(new Error
                {
                    Code = ErrorCodes.Security.Forbidden,
                    Message = ErrorMessages.Security.Forbidden
                });
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

            var team = await _teamsService.Create(userId, company.CompanyId, dto, false);

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

        public async Task<Response<SignUpStepDTO<ProjectDTO>>> SignUpProject(Guid userId, ProjectCreateDTO dto)
        {
            var step = await _repository.GetRegistrationStepByUserId(userId);
            var stepValidate = ValidateStep<ProjectDTO>(step, RegistrationStep.ProjectCreated);
            if (!stepValidate.IsSuccess)
            {
                return stepValidate;
            }

            if (step.Status != MembershipStatus.Admin)
            {
                return new ForbiddenErrorResponse<SignUpStepDTO<ProjectDTO>>(new Error
                {
                    Code = ErrorCodes.Security.Forbidden,
                    Message = ErrorMessages.Security.Forbidden
                });
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

            var project = await _projectsService.Create(userId, team.TeamId, dto, false);

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

            if (step.Status == MembershipStatus.SuperAdmin)
            {
                var sendResult = await SendSignUpInvites(userId);
                if (!sendResult.IsSuccess)
                    return sendResult as Response<SignUpStepDTO>;
            }
            else
            {
                await _invitesService.ActivateInvites(step.User.Email);
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
                        Message = ErrorMessages.Business.UserDoesNotExists
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

        private async Task<Response<SignUpStepDTO<UserDTO>>> SignUpUsernameAdmin(Guid id, UserUpdateDTO updateDTO)
        {
            var userResponse = await _usersService.UpdateUser(id, updateDTO);
            var user = userResponse.Data;

            var result = await _repository.SetRegistrationStep(user.Id, RegistrationStep.UsernameEntered);

            return new Response<SignUpStepDTO<UserDTO>>
            {
                Data = new SignUpStepDTO<UserDTO>
                {
                    Data = user,
                    RegistrationStep = _mapper.Map<UserRegistrationStepModel, UserRegistrationStepDTO>(result)
                }
            };
        }

        private async Task<Response<SignUpStepDTO<UserDTO>>> SignUpUsernameMember(Guid id, UserUpdateDTO updateDTO)
        {
            var user = await _usersRepository.GetById(id);
            user.Name = updateDTO.Name;
            user = await _usersRepository.Update(user);

            var result = await _repository.SetRegistrationStep(user.Id, RegistrationStep.ProjectCreated);
            
            return new Response<SignUpStepDTO<UserDTO>>
            {
                Data = new SignUpStepDTO<UserDTO>
                {
                    Data = _mapper.Map<UserModel, UserDTO>(user),
                    RegistrationStep = _mapper.Map<UserRegistrationStepModel, UserRegistrationStepDTO>(result)
                }
            };
        }
        
        public async Task<Response<InviteModel>> GetInviteByCode(string code)
        {
            // var item = await _invitesRepository.GetInviteByCode(code);
            // if (item == null)
            // {
            //     return new SecurityErrorResponse<InviteModel>(new []
            //     {
            //         new Error
            //         {
            //             Code = ErrorCodes.Common.NotFound,
            //             Message = ErrorMessages.Common.NotFound
            //         }
            //     });
            // }
            //
            // return new Response<InviteModel>
            // {
            //     Data = item
            // };
            throw new NotImplementedException();
        }
        
        private async Task<Response> SendSignUpInvites(Guid userId)
        {
            var pageModel = new PageModel {Page = 1, PageSize = 1};
            
            var userCompany = (await _companiesProvider.GetUserCompanies(userId, pageModel))
                .Data
                .Items
                .FirstOrDefault();

            if (userCompany == null)
            {
                return new BusinessConflictErrorResponse(new[]
                {
                    new Error
                    {
                        Code = ErrorCodes.Common.NotFound,
                        Message = ErrorMessages.Common.NotFound,
                        Field = ErrorFields.Company.Id
                    },
                    new Error
                    {
                        Code = ErrorCodes.Common.NotFound,
                        Message = ErrorMessages.Common.NotFound,
                        Field = ErrorFields.User.Id
                    }
                });
            }

            var userTeam = (await _teamsProvider.GetUserTeams(userId, pageModel, userCompany.CompanyId))
                .Data
                .Items
                .FirstOrDefault();

            if (userTeam == null)
            {
                return new BusinessConflictErrorResponse(new[]
                {
                    new Error
                    {
                        Code = ErrorCodes.Common.NotFound,
                        Message = ErrorMessages.Common.NotFound,
                        Field = ErrorFields.Team.Id
                    },
                    new Error
                    {
                        Code = ErrorCodes.Common.NotFound,
                        Message = ErrorMessages.Common.NotFound,
                        Field = ErrorFields.User.Id
                    }
                });
            }

            var userProject = (await _projectsProvider.GetUserProjects(userId, pageModel, teamId: userTeam.TeamId))
                .Data
                .Items
                .FirstOrDefault();

            if (userProject == null)
            {
                return new BusinessConflictErrorResponse(new[]
                {
                    new Error
                    {
                        Code = ErrorCodes.Common.NotFound,
                        Message = ErrorMessages.Common.NotFound,
                        Field = ErrorFields.Project.Id
                    },
                    new Error
                    {
                        Code = ErrorCodes.Common.NotFound,
                        Message = ErrorMessages.Common.NotFound,
                        Field = ErrorFields.User.Id
                    }
                });
            }

            throw new NotImplementedException();
            // var allInvites = (await _invitesRepository.GetInvites(
            //     authorId: userId, 
            //     activated: false,
            //     isSent: false)).ToList();
            //
            // var emailsDistinct = allInvites.Select(x => x.Email).Distinct().ToList();
            //
            // if (emailsDistinct.Count == 0)
            //     return new Response();
            //
            // foreach (var email in emailsDistinct)
            // {
            //     await _invitesService.SendInvite(email);
            // }
            //
            // return new Response();
        }
    }
}