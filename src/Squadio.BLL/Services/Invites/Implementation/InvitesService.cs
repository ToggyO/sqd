using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Mapper;
using Squadio.BLL.Providers.Codes;
using Squadio.BLL.Services.Email;
using Squadio.Common.Models.Email;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Companies;
using Squadio.DAL.Repository.CompaniesUsers;
using Squadio.DAL.Repository.Invites;
using Squadio.DAL.Repository.Projects;
using Squadio.DAL.Repository.ProjectsUsers;
using Squadio.DAL.Repository.Teams;
using Squadio.DAL.Repository.TeamsUsers;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Invites;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Invites;

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
        private readonly IUsersRepository _usersRepository;
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
            , IUsersRepository usersRepository
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
            _usersRepository = usersRepository;
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
                    email);
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
                    email);
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
                    email);
                result.Add(itemResult.Data);
            }

            return new Response<IEnumerable<InviteDTO>>
            {
                Data = result
            };
        }

        public async Task<Response> CancelInvite(Guid entityId, Guid authorId, CancelInvitesDTO dto)
        {
            var invites = (await _repository.GetInvites(entityId))
                .ToList();
            if (invites.Count == 0)
                return new Response();

            var type = invites.First().EntityType;

            switch (type)
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
                    break;
            }

            await _repository.ActivateInvites(entityId, dto.Emails);
            
            return new Response();
        }

        public async Task<Response> AcceptInvite(Guid userId, string code)
        {
            var invite = await _repository.GetInviteByCode(code);
            
            if (invite == null || invite?.Activated == true)
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
            
            var user = await _usersRepository.GetById(userId);
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

            switch (invite.EntityType)
            {
                case EntityType.Company:
                    return await AcceptInviteToCompany(invite, user);
                case EntityType.Team:
                    return await AcceptInviteToTeam(invite, user);
                case EntityType.Project:
                    return await AcceptInviteToProject(invite, user);
            }
            
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

        private async Task<Response<InviteDTO>> InviteToCompany(string authorName, string companyName, Guid companyId, string email)
        {
            var code = _codeProvider.GenerateCodeAsGuid();
            var invite = new InviteModel
            {
                Email = email,
                Activated = false,
                CreatedDate = DateTime.UtcNow,
                Code = code,
                EntityId = companyId,
                EntityType = EntityType.Company
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
            catch {}

            return new Response<InviteDTO>
            {
                Data = result
            };
        }

        private async Task<Response<InviteDTO>> InviteToTeam(string authorName, string teamName, Guid teamId, string email)
        {
            var code = _codeProvider.GenerateCodeAsGuid();
            var invite = new InviteModel
            {
                Email = email,
                Activated = false,
                CreatedDate = DateTime.UtcNow,
                Code = code,
                EntityId = teamId,
                EntityType = EntityType.Team
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
            catch {}

            return new Response<InviteDTO>
            {
                Data = result
            };
        }

        private async Task<Response<InviteDTO>> InviteToProject(string authorName, string projectName, Guid projectId, string email)
        {
            var code = _codeProvider.GenerateCodeAsGuid();
            var invite = new InviteModel
            {
                Email = email,
                Activated = false,
                CreatedDate = DateTime.UtcNow,
                Code = code,
                EntityId = projectId,
                EntityType = EntityType.Project
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
            catch {}

            return new Response<InviteDTO>
            {
                Data = result
            };
        }
        
        private async Task<Response> AcceptInviteToCompany(InviteModel invite, UserModel user)
        {
            if (invite?.EntityType != EntityType.Company)
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
            
            var company = await _companiesRepository.GetById(invite.EntityId);
            if (company == null)
            {
                return new BusinessConflictErrorResponse(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Common.NotFound,
                        Message = ErrorMessages.Common.NotFound,
                        Field = ErrorFields.Company.Id
                    }
                });
            }

            var companyUser = await _companiesUsersRepository.GetCompanyUser(company.Id, user.Id);
            if (companyUser != null)
            {
                await _repository.ActivateInvite(invite.Id);
                return new Response();
            }
            
            await _companiesUsersRepository.AddCompanyUser(company.Id, user.Id, UserStatus.Member);
            await _repository.ActivateInvite(invite.Id);
            return new Response();
        }

        private async Task<Response> AcceptInviteToTeam(InviteModel invite, UserModel user)
        {
            if (invite?.EntityType != EntityType.Team)
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
            
            var team = await _teamsRepository.GetById(invite.EntityId);
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

            var teamUser = await _teamsUsersRepository.GetTeamUser(team.Id, user.Id);
            if(teamUser != null)
            {
                await _repository.ActivateInvite(invite.Id);
                return new Response();
            }

            var companyUser = await _companiesUsersRepository.GetCompanyUser(team.CompanyId, user.Id);
            if (companyUser == null)
            {
                var company = await _companiesRepository.GetById(team.CompanyId);
                if (company == null)
                {
                    return new BusinessConflictErrorResponse(new []
                    {
                        new Error
                        {
                            Code = ErrorCodes.Common.NotFound,
                            Message = ErrorMessages.Common.NotFound,
                            Field = ErrorFields.Company.Id
                        }
                    });
                }

                await _companiesUsersRepository.AddCompanyUser(company.Id, user.Id, UserStatus.Member);
            }
            
            await _teamsUsersRepository.AddTeamUser(team.Id, user.Id, UserStatus.Member);
            await _repository.ActivateInvite(invite.Id);

            return new Response();
        }

        private async Task<Response> AcceptInviteToProject(InviteModel invite, UserModel user)
        {
            if (invite?.EntityType != EntityType.Project)
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
            
            var project = await _projectsRepository.GetById(invite.EntityId);
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

            var projectUser = await _projectsUsersRepository.GetProjectUser(project.Id, user.Id);
            if (projectUser != null)
            {
                await _repository.ActivateInvite(invite.Id);
                return new Response();
            }

            var teamUser = await _teamsUsersRepository.GetTeamUser(project.TeamId, user.Id);
            if (teamUser == null)
            {
                var team = await _teamsRepository.GetById(project.TeamId);
                if (team == null)
                {
                    return new BusinessConflictErrorResponse(new[]
                    {
                        new Error
                        {
                            Code = ErrorCodes.Common.NotFound,
                            Message = ErrorMessages.Common.NotFound,
                            Field = ErrorFields.Team.Id
                        }
                    });
                }

                var companyUser = await _companiesUsersRepository.GetCompanyUser(team.CompanyId, user.Id);
                if (companyUser == null)
                {
                    var company = await _companiesRepository.GetById(team.CompanyId);
                    if (company == null)
                    {
                        return new BusinessConflictErrorResponse(new[]
                        {
                            new Error
                            {
                                Code = ErrorCodes.Common.NotFound,
                                Message = ErrorMessages.Common.NotFound,
                                Field = ErrorFields.Company.Id
                            }
                        });
                    }

                    await _companiesUsersRepository.AddCompanyUser(company.Id, user.Id, UserStatus.Member);
                }
                
                await _teamsUsersRepository.AddTeamUser(project.TeamId, user.Id, UserStatus.Member);
            }

            await _projectsUsersRepository.AddProjectUser(project.Id, user.Id, UserStatus.Member);
            await _repository.ActivateInvite(invite.Id);
            
            return new Response();
        }
    }
}