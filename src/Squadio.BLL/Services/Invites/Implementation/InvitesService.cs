using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Mapper;
using Squadio.BLL.Services.Email;
using Squadio.Common.Models.Email;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Companies;
using Squadio.DAL.Repository.CompaniesUsers;
using Squadio.DAL.Repository.Invites;
using Squadio.DAL.Repository.ProjectsUsers;
using Squadio.DAL.Repository.Teams;
using Squadio.DAL.Repository.TeamsUsers;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Invites;
using Squadio.DTO.Invites;

namespace Squadio.BLL.Services.Invites.Implementation
{
    public class InvitesService : IInvitesService
    {
        #region Repository variables and constructor
        
        private readonly IInvitesRepository _repository;
        private readonly IEmailService<InviteToTeamEmailModel> _inviteToTeamMailService;
        private readonly IEmailService<InviteToProjectEmailModel> _inviteToProjectMailService;
        private readonly IUsersRepository _usersRepository;
        private readonly ICompaniesRepository _companiesRepository;
        private readonly ICompaniesUsersRepository _companiesUsersRepository;
        private readonly ITeamsRepository _teamsRepository;
        private readonly ITeamsUsersRepository _teamsUsersRepository;
        private readonly IProjectsUsersRepository _projectsUsersRepository;
        private readonly IMapper _mapper;
        public InvitesService(IInvitesRepository repository
            , IEmailService<InviteToTeamEmailModel> inviteToTeamMailService
            , IEmailService<InviteToProjectEmailModel> inviteToProjectMailService
            , IUsersRepository usersRepository
            , ICompaniesRepository companiesRepository
            , ICompaniesUsersRepository companiesUsersRepository
            , ITeamsRepository teamsRepository
            , ITeamsUsersRepository teamsUsersRepository
            , IProjectsUsersRepository projectsUsersRepository
            , IMapper mapper)
        {
            _repository = repository;
            _inviteToTeamMailService = inviteToTeamMailService;
            _inviteToProjectMailService = inviteToProjectMailService;
            _usersRepository = usersRepository;
            _companiesRepository = companiesRepository;
            _companiesUsersRepository = companiesUsersRepository;
            _teamsRepository = teamsRepository;
            _teamsUsersRepository = teamsUsersRepository;
            _projectsUsersRepository = projectsUsersRepository;
            _mapper = mapper;
        }
        
        #endregion

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

        public async Task<Response<InviteDTO>> InviteToTeam(string authorName, string teamName, Guid teamId, string email)
        {
            var invite = await _repository.CreateTeamInvite(email, teamId);
            var result = _mapper.Map<InviteModel, InviteDTO>(invite);

            try
            {
                await _inviteToTeamMailService.Send(new InviteToTeamEmailModel
                {
                    To = email,
                    AuthorName = authorName,
                    Code = invite.Code,
                    TeamId = teamId.ToString(),
                    TeamName = teamName
                });
                result.IsSent = true;
            }
            catch
            {
                result.IsSent = false;
            }

            return new Response<InviteDTO>
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

        public async Task<Response<InviteDTO>> InviteToProject(string authorName, string projectName, Guid projectId, string email)
        {
            var invite = await _repository.CreateProjectInvite(email, projectId);
            var result = _mapper.Map<InviteModel, InviteDTO>(invite);

            try
            {
                await _inviteToProjectMailService.Send(new InviteToProjectEmailModel
                {
                    To = email,
                    AuthorName = authorName,
                    Code = invite.Code,
                    ProjectId = projectId.ToString(),
                    ProjectName = projectName
                });
            }
            catch
            {
                result.IsSent = false;
            }

            return new Response<InviteDTO>
            {
                Data = result
            };
        }

        public async Task<Response> AcceptInviteToTeam(Guid userId, Guid teamId, string code)
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

            var teamUser = await _teamsUsersRepository.GetTeamUser(teamId, userId);
            if(teamUser != null)
            {
                await _repository.ActivateInvite(invite.Id);
                return new Response();
            }
            
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

            var companyUser = await _companiesUsersRepository.GetCompanyUser(team.CompanyId, userId);
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

                await _companiesUsersRepository.AddCompanyUser(company.Id, userId, UserStatus.Member);
            }
            
            await _teamsUsersRepository.AddTeamUser(teamId, userId, UserStatus.Member);
            await _repository.ActivateInvite(invite.Id);

            return new Response();
        }

        public async Task<Response> AcceptInviteToProject(Guid userId, Guid projectId, string code)
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

            var projectUser = await _projectsUsersRepository.GetProjectUser(projectId, userId);
            if (projectUser != null)
            {
                await _repository.ActivateInvite(invite.Id);
                return new Response();
            }

            await _projectsUsersRepository.AddProjectUser(projectId, userId, UserStatus.Member);
            await _repository.ActivateInvite(invite.Id);
            
            return new Response();
        }
    }
}