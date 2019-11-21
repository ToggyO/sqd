using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapper;
using Squadio.BLL.Providers.Codes;
using Squadio.BLL.Providers.Users;
using Squadio.BLL.Services.Email;
using Squadio.BLL.Services.Users;
using Squadio.Common.Models.Email;
using Squadio.Common.Models.Errors;
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
        private readonly IEmailService<InviteToTeamEmailModel> _inviteToTeamMailService;
        private readonly IEmailService<InviteToProjectEmailModel> _inviteToProjectMailService;
        private readonly IEmailService<InviteToCompanyEmailModel> _inviteToCompanyMailService;
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
        public InvitesService(IInvitesRepository repository
            , IEmailService<InviteToTeamEmailModel> inviteToTeamMailService
            , IEmailService<InviteToProjectEmailModel> inviteToProjectMailService
            , IEmailService<InviteToCompanyEmailModel> inviteToCompanyMailService
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
            , IMapper mapper)
        {
            _repository = repository;
            _inviteToTeamMailService = inviteToTeamMailService;
            _inviteToProjectMailService = inviteToProjectMailService;
            _inviteToCompanyMailService = inviteToCompanyMailService;
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
        }
        
        #endregion


        public async Task<Response<IEnumerable<InviteDTO>>> InviteToCompany(Guid companyId, Guid authorId, CreateInvitesDTO dto)
        {
            var companyUser = await _companiesUsersRepository.GetCompanyUser(companyId, authorId);
            if (companyUser == null || companyUser?.Status == UserStatus.Member)
            {
                return new ForbiddenErrorResponse<IEnumerable<InviteDTO>>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.Forbidden,
                        Message = ErrorMessages.Security.Forbidden,
                    }
                });
            }

            var result = new List<InviteDTO>();
            foreach (var email in dto.Emails)
            {
                var itemResult = await InviteToCompany(
                    companyUser.User.Name,
                    companyUser.Company.Name,
                    companyUser.CompanyId,
                    email,
                    authorId);
                result.Add(itemResult.Data);
            }

            return new Response<IEnumerable<InviteDTO>>
            {
                Data = result
            };
        }

        public async Task<Response<IEnumerable<InviteDTO>>> InviteToTeam(Guid teamId, Guid authorId, CreateInvitesDTO dto)
        {
            var teamUser = await _teamsUsersRepository.GetTeamUser(teamId, authorId);
            if (teamUser == null || teamUser?.Status == UserStatus.Member)
            {
                return new ForbiddenErrorResponse<IEnumerable<InviteDTO>>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.Forbidden,
                        Message = ErrorMessages.Security.Forbidden,
                    }
                });
            }

            var result = new List<InviteDTO>();
            foreach (var email in dto.Emails)
            {
                var itemResult = await InviteToTeam(
                    teamUser.User.Name,
                    teamUser.Team.Name,
                    teamUser.TeamId,
                    email,
                    authorId);
                result.Add(itemResult.Data);
            }

            return new Response<IEnumerable<InviteDTO>>
            {
                Data = result
            };
        }

        public async Task<Response<IEnumerable<InviteDTO>>> InviteToProject(Guid projectId, Guid authorId, CreateInvitesDTO dto)
        {
            
            var projectUser = await _projectsUsersRepository.GetProjectUser(projectId, authorId);
            if (projectUser == null || projectUser?.Status == UserStatus.Member)
            {
                return new ForbiddenErrorResponse<IEnumerable<InviteDTO>>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.Forbidden,
                        Message = ErrorMessages.Security.Forbidden,
                    }
                });
            }

            var result = new List<InviteDTO>();
            foreach (var email in dto.Emails)
            {
                var itemResult = await InviteToProject(
                    projectUser.User.Name,
                    projectUser.Project.Name,
                    projectUser.ProjectId,
                    email,
                    authorId);
                result.Add(itemResult.Data);
            }

            return new Response<IEnumerable<InviteDTO>>
            {
                Data = result
            };
        }

        public async Task<Response> CancelInvite(Guid entityId, Guid authorId, CancelInvitesDTO dto, EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Company:
                    var companyUser = await _companiesUsersRepository.GetCompanyUser(entityId, authorId);
                    if (companyUser == null || companyUser?.Status == UserStatus.Member)
                    {
                        return new ForbiddenErrorResponse<IEnumerable<InviteDTO>>(new []
                        {
                            new Error
                            {
                                Code = ErrorCodes.Security.Forbidden,
                                Message = ErrorMessages.Security.Forbidden,
                            }
                        });
                    }
                    await _companiesUsersRepository.DeleteCompanyUsers(entityId, dto.Emails);
                    break;
                case EntityType.Team:
                    var teamUser = await _teamsUsersRepository.GetTeamUser(entityId, authorId);
                    if (teamUser == null || teamUser?.Status == UserStatus.Member)
                    {
                        return new ForbiddenErrorResponse<IEnumerable<InviteDTO>>(new []
                        {
                            new Error
                            {
                                Code = ErrorCodes.Security.Forbidden,
                                Message = ErrorMessages.Security.Forbidden,
                            }
                        });
                    }
                    await _teamsUsersRepository.DeleteTeamUsers(entityId, dto.Emails);
                    break;
                case EntityType.Project:
                    var projectUser = await _projectsUsersRepository.GetProjectUser(entityId, authorId);
                    if (projectUser == null || projectUser?.Status == UserStatus.Member)
                    {
                        return new ForbiddenErrorResponse<IEnumerable<InviteDTO>>(new []
                        {
                            new Error
                            {
                                Code = ErrorCodes.Security.Forbidden,
                                Message = ErrorMessages.Security.Forbidden,
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
                return new ForbiddenErrorResponse(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.Forbidden,
                        Message = ErrorMessages.Security.Forbidden
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

        private async Task<Response<InviteDTO>> InviteToCompany(string authorName, string companyName, Guid companyId, string email, Guid authorId)
        {
            var code = _codeProvider.GenerateCodeAsGuid();
            
            var user = (await _usersProvider.GetByEmail(email)).Data;
            if (user == null)
            {
                var createUserDTO = new UserCreateDTO()
                {
                    Email = email,
                    Step = RegistrationStep.New
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
                return new Response<InviteDTO>();
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
            var result = _mapper.Map<InviteModel, InviteDTO>(invite);

            try
            {
                _inviteToCompanyMailService.Send(new InviteToCompanyEmailModel()
                {
                    To = email,
                    AuthorName = authorName,
                    Code = invite.Code,
                    CompanyName = companyName
                });
            }
            catch
            {
                // ignored
            }

            return new Response<InviteDTO>
            {
                Data = result
            };
        }

        private async Task<Response<InviteDTO>> InviteToTeam(string authorName, string teamName, Guid teamId, string email, Guid authorId)
        {
            var code = _codeProvider.GenerateCodeAsGuid();
            
            var user = (await _usersProvider.GetByEmail(email)).Data;
            if (user == null)
            {
                var createUserDTO = new UserCreateDTO()
                {
                    Email = email,
                    Step = RegistrationStep.New
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
                return new Response<InviteDTO>();
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
            var result = _mapper.Map<InviteModel, InviteDTO>(invite);

            try
            {
                _inviteToTeamMailService.Send(new InviteToTeamEmailModel
                {
                    To = email,
                    AuthorName = authorName,
                    Code = invite.Code,
                    TeamName = teamName
                });
            }
            catch
            {
                // ignored
            }

            return new Response<InviteDTO>
            {
                Data = result
            };
        }

        private async Task<Response<InviteDTO>> InviteToProject(string authorName, string projectName, Guid projectId, string email, Guid authorId)
        {
            var code = _codeProvider.GenerateCodeAsGuid();
            
            var user = (await _usersProvider.GetByEmail(email)).Data;
            if (user == null)
            {
                var createUserDTO = new UserCreateDTO()
                {
                    Email = email,
                    Step = RegistrationStep.New
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
                return new Response<InviteDTO>();
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
            var result = _mapper.Map<InviteModel, InviteDTO>(invite);

            try
            {
                _inviteToProjectMailService.Send(new InviteToProjectEmailModel
                {
                    To = email,
                    AuthorName = authorName,
                    Code = invite.Code,
                    ProjectName = projectName
                });
            }
            catch
            {
                // ignored
            }

            return new Response<InviteDTO>
            {
                Data = result
            };
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