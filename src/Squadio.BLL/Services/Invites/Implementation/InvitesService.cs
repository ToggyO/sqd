using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapper;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Squadio.BLL.Providers.Codes;
using Squadio.BLL.Providers.Users;
using Squadio.BLL.Services.Rabbit;
using Squadio.BLL.Services.Users;
using Squadio.Common.Models.Email;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.ChangePassword;
using Squadio.DAL.Repository.Companies;
using Squadio.DAL.Repository.CompaniesUsers;
using Squadio.DAL.Repository.Invites;
using Squadio.DAL.Repository.Projects;
using Squadio.DAL.Repository.ProjectsUsers;
using Squadio.DAL.Repository.SignUp;
using Squadio.DAL.Repository.Teams;
using Squadio.DAL.Repository.TeamsUsers;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Invites;
using Squadio.DTO.Invites;
using Squadio.DTO.Users;

namespace Squadio.BLL.Services.Invites.Implementation
{
    public class InvitesService : IInvitesService
    {
        #region Repository variables and constructor
        
        private readonly IInvitesRepository _repository;
        private readonly IRabbitService _rabbitService;
        private readonly ICompaniesRepository _companiesRepository;
        private readonly ICompaniesUsersRepository _companiesUsersRepository;
        private readonly ITeamsRepository _teamsRepository;
        private readonly ITeamsUsersRepository _teamsUsersRepository;
        private readonly IProjectsRepository _projectsRepository;
        private readonly IProjectsUsersRepository _projectsUsersRepository;
        private readonly ICodeProvider _codeProvider;
        private readonly IUsersService _usersService;
        private readonly IUsersProvider _usersProvider;
        private readonly ISignUpRepository _signUpRepository;
        private readonly IChangePasswordRequestRepository _changePasswordRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<InvitesService> _logger;
        
        public InvitesService(IInvitesRepository repository
            , IRabbitService rabbitService
            , ICompaniesRepository companiesRepository
            , ICompaniesUsersRepository companiesUsersRepository
            , ITeamsRepository teamsRepository
            , ITeamsUsersRepository teamsUsersRepository
            , IProjectsRepository projectsRepository
            , IProjectsUsersRepository projectsUsersRepository
            , ICodeProvider codeProvider
            , IUsersService usersService
            , IUsersProvider usersProvider
            , ISignUpRepository signUpRepository
            , IChangePasswordRequestRepository changePasswordRepository
            , IMapper mapper
            , ILogger<InvitesService> logger)
        {
            _repository = repository;
            _rabbitService = rabbitService;
            _companiesRepository = companiesRepository;
            _companiesUsersRepository = companiesUsersRepository;
            _teamsRepository = teamsRepository;
            _teamsUsersRepository = teamsUsersRepository;
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
        
        #endregion


        public async Task<Response> InviteToCompany(Guid companyId, Guid authorId, CreateInvitesDTO dto, bool sendInvites = true)
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
                        companyUser.User.Name,
                        companyUser.Company.Name,
                        createInviteResult.Item2);
                }
            }

            return new Response();
        }

        public async Task<Response> InviteToTeam(Guid teamId, Guid authorId, CreateInvitesDTO dto, bool sendInvites = true)
        {
            var teamUser = await _teamsUsersRepository.GetTeamUser(teamId, authorId);
            if (teamUser == null || teamUser?.Status == UserStatus.Member)
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
                var createInviteResult = await CreateInviteToTeam(
                    teamUser.TeamId,
                    email,
                    authorId);
                if (createInviteResult != null && sendInvites)
                {
                    await SendTeamInvite(
                        createInviteResult.Item1,
                        teamUser.User.Name,
                        teamUser.Team.Name,
                        createInviteResult.Item2);
                }
            }

            return new Response();
        }

        public async Task<Response> InviteToProject(Guid projectId, Guid authorId, CreateInvitesDTO dto, bool sendInvites = true)
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
                        projectUser.User.Name,
                        projectUser.Project.Name,
                        createInviteResult.Item2);
                }
            }
            
            return new Response();
        }

        public async Task<Response> CancelInvite(Guid entityId, Guid authorId, CancelInvitesDTO dto, EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Company:
                    var companyUser = await _companiesUsersRepository.GetCompanyUser(entityId, authorId);
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
                    await _companiesUsersRepository.DeleteCompanyUsers(entityId, dto.Emails);
                    break;
                case EntityType.Team:
                    var teamUser = await _teamsUsersRepository.GetTeamUser(entityId, authorId);
                    if (teamUser == null || teamUser?.Status == UserStatus.Member)
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
                    await _teamsUsersRepository.DeleteTeamUsers(entityId, dto.Emails);
                    break;
                case EntityType.Project:
                    var projectUser = await _projectsUsersRepository.GetProjectUser(entityId, authorId);
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
                    await _projectsUsersRepository.DeleteProjectUsers(entityId, dto.Emails);
                    break;
            }

            await _repository.ActivateInvites(entityId, dto.Emails);
            
            return new Response();
        }

        public async Task<Response> AcceptInvite(Guid userId, string code, EntityType entityType)
        {
            var invite = await _repository.GetInviteByCode(code);
            
            if (invite == null || invite?.Activated == true || invite?.EntityType != entityType)
            {
                return new SecurityErrorResponse(new []
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
                return new BusinessConflictErrorResponse(new []
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
                return new PermissionDeniedErrorResponse(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.PermissionDenied,
                        Message = ErrorMessages.Security.PermissionDenied
                    }
                });
            }

            var result = new Response();

            switch (invite.EntityType)
            {
                case EntityType.Company:
                    result = await AcceptInviteToCompany(invite.EntityId, user.Id);
                    await _repository.ActivateInvite(invite.Id);
                    break;
                case EntityType.Team:
                    result = await AcceptInviteToTeam(invite.EntityId, user.Id);
                    await _repository.ActivateInvite(invite.Id);
                    break;
                case EntityType.Project:
                    result = await AcceptInviteToProject(invite.EntityId, user.Id);
                    await _repository.ActivateInvite(invite.Id);
                    break;
                default:
                    return new BusinessConflictErrorResponse(new []
                    {
                        new Error
                        {
                            Code = ErrorCodes.Common.NotFound,
                            Message = ErrorMessages.Common.NotFound,
                            Field = "EntityType"
                        }
                    });
            }

            return result;
        }

        public async Task<Response> SendSignUpInvites(Guid userId)
        {
            var pageModel = new PageModel {Page = 1, PageSize = 1};

            var userCompany = (await _companiesUsersRepository.GetUserCompanies(userId, pageModel))
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

            var userTeam = (await _teamsUsersRepository.GetUserTeams(userId, pageModel, userCompany.CompanyId))
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

            var userProject =
                (await _projectsUsersRepository.GetUserProjects(userId, pageModel, teamId: userTeam.TeamId))
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

            var allInvites = (await _repository.GetInvites(
                authorId: userId, 
                activated: false)).ToList();

            if (allInvites.Count == 0)
                return new Response();

            List<string> usedEmails;
            List<Guid> redundantInvitesIds = new List<Guid>();

            var projectInvites = allInvites.Where(x => x.EntityType == EntityType.Project).ToList();
            allInvites.RemoveAll(x => x.EntityType == EntityType.Project);
            usedEmails = projectInvites.Select(x=>x.Email).Distinct().ToList();
            redundantInvitesIds.AddRange(allInvites.Where(x => usedEmails.Any(y => y.ToUpper() == x.Email.ToUpper())).Select(x=>x.Id));
            allInvites.RemoveAll(x => redundantInvitesIds.Any(y => y == x.Id));

            foreach (var invite in projectInvites)
            {
                await SendProjectInvite(
                    invite,
                    userProject.User.Name,
                    userProject.Project.Name,
                    false);
            }

            var teamInvites = allInvites.Where(x => x.EntityType == EntityType.Team).ToList();
            allInvites.RemoveAll(x => x.EntityType == EntityType.Team);
            usedEmails = teamInvites.Select(x=>x.Email).Distinct().ToList();
            redundantInvitesIds.AddRange(allInvites.Where(x => usedEmails.Any(y => y.ToUpper() == x.Email.ToUpper())).Select(x=>x.Id));
            allInvites.RemoveAll(x => redundantInvitesIds.Any(y => y == x.Id));

            foreach (var invite in teamInvites)
            {
                await SendTeamInvite(
                    invite,
                    userTeam.User.Name,
                    userTeam.Team.Name,
                    false);
            }

            var companyInvites = allInvites.Where(x => x.EntityType == EntityType.Company).ToList();
            allInvites.RemoveAll(x => x.EntityType == EntityType.Company);
            usedEmails = companyInvites.Select(x=>x.Email).Distinct().ToList();
            redundantInvitesIds.AddRange(allInvites.Where(x => usedEmails.Any(y => y.ToUpper() == x.Email.ToUpper())).Select(x=>x.Id));
            allInvites.RemoveAll(x => redundantInvitesIds.Any(y => y == x.Id));

            foreach (var invite in companyInvites)
            {
                await SendCompanyInvite(
                    invite,
                    userCompany.User.Name,
                    userCompany.Company.Name,
                    false);
            }

            if (redundantInvitesIds.Count > 0)
            {
                await _repository.DeleteInvites(redundantInvitesIds);
                _logger.LogInformation($"Removed {redundantInvitesIds.Count} redundant invites while user registered (userid: {userId})");
            }

            if (allInvites.Count > 0)
            {
                _logger.LogWarning($"Some invites not sent while user registered (userid: {userId})");
            }

            return new Response();
        }

        private async Task<Tuple<InviteModel, bool>> CreateInviteToCompany(Guid companyId, string email, Guid authorId)
        {
            var code = _codeProvider.GenerateCodeAsGuid();
            
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
            }

            var signUpStep = await _signUpRepository.GetRegistrationStepByUserId(user.Id);
            if (signUpStep.Step == RegistrationStep.New)
            {
                await _changePasswordRepository.AddRequest(user.Id, code);
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
                Activated = false,
                CreatedDate = DateTime.UtcNow,
                Code = code,
                EntityId = companyId,
                EntityType = EntityType.Company,
                CreatorId = authorId
            };
            
            invite = await _repository.CreateInvite(invite);

            return new Tuple<InviteModel, bool>(invite, signUpStep.Step != RegistrationStep.New);
        }
        
        private async Task<Tuple<InviteModel, bool>> CreateInviteToTeam(Guid teamId, string email, Guid authorId)
        {
            var code = _codeProvider.GenerateCodeAsGuid();
            
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
            }

            var signUpStep = await _signUpRepository.GetRegistrationStepByUserId(user.Id);
            if (signUpStep.Step == RegistrationStep.New)
            {
                await _changePasswordRepository.AddRequest(user.Id, code);
            }
            
            var teamUser = await _teamsUsersRepository.GetTeamUser(teamId, user.Id);
            if (teamUser != null)
            {
                return null;
            }
            
            await _teamsUsersRepository.AddTeamUser(teamId, user.Id, UserStatus.Pending);
            
            var invite = new InviteModel
            {
                Email = email,
                Activated = false,
                CreatedDate = DateTime.UtcNow,
                Code = code,
                EntityId = teamId,
                EntityType = EntityType.Team,
                CreatorId = authorId
            };
            
            invite = await _repository.CreateInvite(invite);
            
            return new Tuple<InviteModel, bool>(invite, signUpStep.Step != RegistrationStep.New);
        }

        private async Task<Tuple<InviteModel, bool>> CreateInviteToProject(Guid projectId, string email, Guid authorId)
        {
            var code = _codeProvider.GenerateCodeAsGuid();
            
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
            }

            var signUpStep = await _signUpRepository.GetRegistrationStepByUserId(user.Id);
            if (signUpStep.Step == RegistrationStep.New)
            {
                await _changePasswordRepository.AddRequest(user.Id, code);
            }
            
            var projectUser = await _projectsUsersRepository.GetProjectUser(projectId, user.Id);
            if (projectUser != null)
            {
                return null;
            }
            
            await _projectsUsersRepository.AddProjectUser(projectId, user.Id, UserStatus.Pending);
            
            var invite = new InviteModel
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
            
            return new Tuple<InviteModel, bool>(invite, signUpStep.Step != RegistrationStep.New);
        }
        
        private async Task SendCompanyInvite(InviteModel model, string authorName, string companyName, bool isAlreadyRegistered)
        {
            try
            {
                await _rabbitService.Send(new InviteToCompanyEmailModel()
                {
                    To = model.Email,
                    AuthorName = authorName,
                    Code = model.Code,
                    CompanyName = companyName,
                    IsAlreadyRegistered = isAlreadyRegistered
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{model.Email}][{model.Code}] : {ex.Message}");
            }
        }
        
        private async Task SendTeamInvite(InviteModel model, string authorName, string teamName, bool isAlreadyRegistered)
        {
            try
            {
                await _rabbitService.Send(new InviteToTeamEmailModel()
                {
                    To = model.Email,
                    AuthorName = authorName,
                    Code = model.Code,
                    TeamName = teamName,
                    IsAlreadyRegistered = isAlreadyRegistered
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{model.Email}][{model.Code}] : {ex.Message}");
            }
        }
        
        private async Task SendProjectInvite(InviteModel model, string authorName, string projectName, bool isAlreadyRegistered)
        {
            try
            {
                await _rabbitService.Send(new InviteToProjectEmailModel()
                {
                    To = model.Email,
                    AuthorName = authorName,
                    Code = model.Code,
                    ProjectName = projectName,
                    IsAlreadyRegistered = isAlreadyRegistered
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{model.Email}][{model.Code}] : {ex.Message}");
            }
        }
        
        private async Task<Response> AcceptInviteToCompany(Guid companyId, Guid userId)
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

        private async Task<Response> AcceptInviteToTeam(Guid teamId, Guid userId)
        {
            var team = await _teamsRepository.GetById(teamId);
            if (team == null)
            {
                return new BusinessConflictErrorResponse(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Common.NotFound,
                        Message = ErrorMessages.Common.NotFound,
                        Field = ErrorFields.Team.Id
                    }
                });
            }

            var companyAccept = await AcceptInviteToCompany(team.CompanyId, userId);
            if (!companyAccept.IsSuccess) return companyAccept;

            var teamUser = await _teamsUsersRepository.GetTeamUser(team.Id, userId);
            if (teamUser == null)
                await _teamsUsersRepository.AddTeamUser(team.Id, userId, UserStatus.Member);
            else if (teamUser.Status == UserStatus.Pending)
                await _teamsUsersRepository.ChangeStatusTeamUser(team.Id, userId, UserStatus.Member);


            return new Response();
        }

        private async Task<Response> AcceptInviteToProject(Guid projectId, Guid userId)
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

            var teamAccept = await AcceptInviteToTeam(project.TeamId, userId);
            if (!teamAccept.IsSuccess) return teamAccept;

            var projectUser = await _projectsUsersRepository.GetProjectUser(project.Id, userId);
            if (projectUser == null)
                await _projectsUsersRepository.AddProjectUser(project.Id, userId, UserStatus.Member);
            else if (projectUser.Status == UserStatus.Pending)
                await _projectsUsersRepository.ChangeStatusProjectUser(project.Id, userId, UserStatus.Member);
            
            return new Response();
        }
    }
}