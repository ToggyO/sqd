using System;
using System.Collections.Generic;
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
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.ChangePassword;
using Squadio.DAL.Repository.Invites;
using Squadio.DAL.Repository.Projects;
using Squadio.DAL.Repository.ProjectsUsers;
using Squadio.DAL.Repository.SignUp;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Invites;
using Squadio.DTO.Invites;
using Squadio.DTO.Users;

namespace Squadio.BLL.Services.Projects.Implementation
{
    public class ProjectInvitesService : IProjectInvitesService
    {
        private readonly IInvitesRepository _repository;
        private readonly ITeamInvitesService _teamInvitesService;
        private readonly IRabbitService _rabbitService;
        private readonly IProjectsRepository _projectsRepository;
        private readonly IProjectsUsersRepository _projectsUsersRepository;
        private readonly ICodeProvider _codeProvider;
        private readonly IUsersService _usersService;
        private readonly IUsersProvider _usersProvider;
        private readonly ISignUpRepository _signUpRepository;
        private readonly IChangePasswordRequestRepository _changePasswordRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProjectInvitesService> _logger;
        
        public ProjectInvitesService(IInvitesRepository repository
            , ITeamInvitesService teamInvitesService
            , IRabbitService rabbitService
            , IProjectsRepository projectsRepository
            , IProjectsUsersRepository projectsUsersRepository
            , ICodeProvider codeProvider
            , IUsersService usersService
            , IUsersProvider usersProvider
            , ISignUpRepository signUpRepository
            , IChangePasswordRequestRepository changePasswordRepository
            , IMapper mapper
            , ILogger<ProjectInvitesService> logger)
        {
            _repository = repository;
            _teamInvitesService = teamInvitesService;
            _rabbitService = rabbitService;
            _projectsRepository = projectsRepository;
            _projectsUsersRepository = projectsUsersRepository;
            _codeProvider = codeProvider;
            _usersService = usersService;
            _usersProvider = usersProvider;
            _signUpRepository = signUpRepository;
            _changePasswordRepository = changePasswordRepository;
            _mapper = mapper;
            _logger = logger;
        }
        
        public async Task<Response> CreateInvite(Guid projectId, Guid authorId, CreateInvitesDTO dto, bool sendInvites = true)
        {
            var projectUser = await _projectsUsersRepository.GetProjectUser(projectId, authorId);
            if (projectUser == null || projectUser?.Status == UserStatus.Member)
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
                var createInviteResult = await CreateInviteToProject(
                    projectUser.ProjectId,
                    email,
                    authorId);
                if (createInviteResult != null && sendInvites)
                {
                    await SendProjectInvite(
                        createInviteResult.Item1,
                        email,
                        projectUser.User.Name,
                        projectUser.Project.Name,
                        createInviteResult.Item2);
                }
            }
            
            return new Response();
        }

        public async Task<Response> CancelInvite(Guid teamId, Guid authorId, CancelInvitesDTO dto)
        {
            var projectUser = await _projectsUsersRepository.GetProjectUser(teamId, authorId);
            if (projectUser == null || projectUser?.Status == UserStatus.Member)
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
            await _projectsUsersRepository.DeleteProjectUsers(teamId, dto.Emails);

            await _repository.ActivateInvites(teamId, dto.Emails);

            return new Response();
        }

        public async Task<Response> AcceptInvite(Guid userId, string code)
        {
            var invite = await _repository.GetInviteByCode(code);

            if (invite == null
                || invite?.Activated == true
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
        
        public async Task<Response> AcceptInvite(Guid projectId, Guid userId)
        {
            var project = await _projectsRepository.GetById(projectId);
            if (project == null)
            {
                return new BusinessConflictErrorResponse(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Common.NotFound,
                        Message = ErrorMessages.Common.NotFound,
                        Field = ErrorFields.Project.Id
                    }
                });
            }

            var teamAccept = await _teamInvitesService.AcceptInvite(project.TeamId, userId);
            if (!teamAccept.IsSuccess) return teamAccept;

            var projectUser = await _projectsUsersRepository.GetProjectUser(project.Id, userId);
            if (projectUser == null)
                await _projectsUsersRepository.AddProjectUser(project.Id, userId, UserStatus.Member);
            else if (projectUser.Status == UserStatus.Pending)
                await _projectsUsersRepository.ChangeStatusProjectUser(project.Id, userId, UserStatus.Member);
            
            return new Response();
        }
        
        private async Task<Tuple<InviteModel, bool>> CreateInviteToProject(Guid projectId, string email, Guid authorId)
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
            
                var projectUser = await _projectsUsersRepository.GetProjectUser(projectId, user.Id);
                if (projectUser != null)
                {
                    return null;
                }
            
                await _projectsUsersRepository.AddProjectUser(projectId, user.Id, UserStatus.Pending);
            
                invite = new InviteModel
                {
                    Email = email,
                    Activated = false,
                    CreatedDate = DateTime.UtcNow,
                    Code = code,
                    EntityId = projectId,
                    EntityType = EntityType.Project,
                    CreatorId = authorId
                };
            
                invite = await _repository.CreateInvite(invite);
            }
            
            return new Tuple<InviteModel, bool>(invite, isAlreadyRegistered);
        }
        
        private async Task SendProjectInvite(InviteModel model, string email, string authorName, string projectName, bool isAlreadyRegistered)
        {
            try
            {
                if (model != null)
                {
                    await _rabbitService.Send(new InviteToProjectEmailModel()
                    {
                        To = email,
                        AuthorName = authorName,
                        Code = model.Code,
                        ProjectName = projectName,
                        IsAlreadyRegistered = isAlreadyRegistered
                    });
                }
                else
                {
                    await _rabbitService.Send(new AddToProjectEmailModel()
                    {
                        To = email,
                        AuthorName = authorName,
                        ProjectName = projectName
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