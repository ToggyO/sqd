using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Squadio.BLL.Providers.Users;
using Squadio.BLL.Services.Invites;
using Squadio.BLL.Services.Users;
using Squadio.Common.Helpers;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Companies;
using Squadio.DAL.Repository.CompaniesUsers;
using Squadio.DAL.Repository.Projects;
using Squadio.DAL.Repository.ProjectsUsers;
using Squadio.DAL.Repository.Teams;
using Squadio.DAL.Repository.TeamsUsers;
using Squadio.Domain.Enums;
using Squadio.DTO.Models.Invites;
using Squadio.DTO.Models.Users;

namespace Squadio.BLL.Services.Membership.Implementations
{
    public class MembershipService : IMembershipService
    {
        #region Init
        
        private readonly ICompaniesUsersRepository _companiesUsersRepository;
        private readonly ITeamsUsersRepository _teamsUsersRepository;
        private readonly IProjectsUsersRepository _projectsUsersRepository;
        private readonly ICompaniesRepository _companiesRepository;
        private readonly ITeamsRepository _teamsRepository;
        private readonly IProjectsRepository _projectsRepository;
        private readonly IUsersService _usersService;
        private readonly IUsersProvider _usersProvider;
        private readonly IInvitesService _invitesService;
        private readonly IMapper _mapper;
        private readonly ILogger<MembershipService> _logger;

        public MembershipService(ICompaniesUsersRepository companiesUsersRepository
            , ITeamsUsersRepository teamsUsersRepository
            , IProjectsUsersRepository projectsUsersRepository
            , ICompaniesRepository companiesRepository
            , ITeamsRepository teamsRepository
            , IProjectsRepository projectsRepository
            , IUsersService usersService
            , IUsersProvider usersProvider
            , IInvitesService invitesService
            , IMapper mapper
            , ILogger<MembershipService> logger
            )
        {
            _companiesUsersRepository = companiesUsersRepository;
            _teamsUsersRepository = teamsUsersRepository;
            _projectsUsersRepository = projectsUsersRepository;
            _companiesRepository = companiesRepository;
            _teamsRepository = teamsRepository;
            _projectsRepository = projectsRepository;
            _usersService = usersService;
            _usersProvider = usersProvider;
            _invitesService = invitesService;
            _mapper = mapper;
            _logger = logger;
        }
        
        #endregion

        public async Task<Response> ApplyInvite(Guid userId, Guid entityId, InviteEntityType entityType)
        {
            var user = await _usersProvider.GetById(userId);
            switch (entityType)
            {
                case InviteEntityType.Company:
                    await AddUserToCompany(entityId, userId, MembershipStatus.Member);
                    break;
                case InviteEntityType.Team:
                    await AddUserToTeam(entityId, userId, MembershipStatus.Member);
                    break;
                case InviteEntityType.Project:
                    await AddUserToProject(entityId, userId, MembershipStatus.Member);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(entityType), entityType, null);
            }

            await _invitesService.RemoveInvite(user.Data.Email, entityId, entityType);
            return new Response();
        }

        public async Task<Response> AddUserToCompany(Guid companyId, Guid userId, MembershipStatus membershipStatus)
        {
            var companyUser = await _companiesUsersRepository.GetCompanyUser(companyId, userId);
            if(companyUser == null)
                await _companiesUsersRepository.AddCompanyUser(companyId, userId, membershipStatus);
            return new Response();
        }

        public async Task<Response> AddUserToTeam(Guid teamId, Guid userId, MembershipStatus membershipStatus)
        {
            var team = await _teamsRepository.GetById(teamId);
            await AddUserToCompany(team.CompanyId, userId, membershipStatus);
            var teamUser = await _teamsUsersRepository.GetTeamUser(teamId, userId);
            if(teamUser == null)
                await _teamsUsersRepository.AddTeamUser(teamId, userId, membershipStatus);
            return new Response();
        }

        public async Task<Response> AddUserToProject(Guid projectId, Guid userId, MembershipStatus membershipStatus)
        {
            var project = await _projectsRepository.GetById(projectId);
            await AddUserToTeam(project.TeamId, userId, membershipStatus);
            var projectUser = await _projectsUsersRepository.GetProjectUser(projectId, userId);
            if(projectUser == null)
                await _projectsUsersRepository.AddProjectUser(projectId, userId, membershipStatus);
            return new Response();
        }

        public async Task<Response> InviteUsers(Guid entityId, InviteEntityType entityType, Guid authorId, IEnumerable<string> emails, bool sendMails = true)
        {
            var distinctEmails = emails.Distinct().ToList();

            var existedUsersEmails = await GetExistedUsersInEntity(entityId, entityType, distinctEmails);
            var notExistedEmails = distinctEmails.Except(existedUsersEmails).ToList();
            var existedInvites = await _invitesService.GetInvites(notExistedEmails, entityId, entityType);

            notExistedEmails = notExistedEmails.Except(existedInvites.Data.Select(x => x.Email)).ToList();

            foreach (var email in notExistedEmails)
            {
                await _invitesService.CreateInvite(email, entityId, entityType, authorId);

                if (sendMails)
                {
                    await _invitesService.SendInvite(email, entityId, entityType);
                }
            }

            return new Response();
        }

        public async Task<Response> DeleteUserFromCompany(Guid companyId, Guid removeUserId, Guid currentUserId)
        {
            var currentCompanyUser = await _companiesUsersRepository.GetCompanyUser(companyId, currentUserId);
        
            if (currentCompanyUser == null || currentCompanyUser?.Status != MembershipStatus.SuperAdmin)
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
            
            var teams = await _teamsRepository.GetTeams(new PageModel()
            {
                Page = 1,
                PageSize = 1000
            }, companyId);
            
            foreach (var team in teams.Items)
            {
                await DeleteUserFromTeam(team.Id, removeUserId, currentUserId, false);
            }
        
            await _companiesUsersRepository.DeleteCompanyUser(companyId, removeUserId);
            
            return new Response();
        }
        
        public async Task<Response> DeleteUserFromTeam(Guid teamId, Guid removeUserId, Guid currentUserId, bool checkAccess = true)
        {
            if (checkAccess)
            {
                var currentTeamUser = await _teamsUsersRepository.GetTeamUser(teamId, currentUserId);
        
                if (currentTeamUser == null || currentTeamUser?.Status != MembershipStatus.SuperAdmin)
                {
                    return new ForbiddenErrorResponse(new[]
                    {
                        new Error
                        {
                            Code = ErrorCodes.Security.Forbidden,
                            Message = ErrorMessages.Security.Forbidden
                        }
                    });
                }
            }
        
            var projects = await _projectsRepository.GetProjects(new PageModel()
            {
                Page = 1,
                PageSize = 1000
            }, teamId);
            
            foreach (var project in projects.Items)
            {
                await DeleteUserFromProject(project.Id, removeUserId, currentUserId, false);
            }
            
            await _teamsUsersRepository.DeleteTeamUser(teamId, removeUserId);
            return new Response();
        }
        
        public async Task<Response> DeleteUserFromProject(Guid projectId, Guid removeUserId, Guid currentUserId, bool checkAccess = true)
        {
            if (checkAccess)
            {
                var currentProjectUser = await _projectsUsersRepository.GetProjectUser(projectId, currentUserId);
        
                if (currentProjectUser == null || currentProjectUser?.Status != MembershipStatus.SuperAdmin)
                {
                    return new ForbiddenErrorResponse(new[]
                    {
                        new Error
                        {
                            Code = ErrorCodes.Security.Forbidden,
                            Message = ErrorMessages.Security.Forbidden
                        }
                    });
                }
            }
            await _projectsUsersRepository.DeleteProjectUser(projectId, removeUserId);
            return new Response();
        }

        private async Task<IEnumerable<string>> GetExistedUsersInEntity(Guid entityId, InviteEntityType entityType, IEnumerable<string> emails)
        {
            List<string> result;
            
            switch (entityType)
            {
                case InviteEntityType.Company:
                    var companyUsers = await _companiesUsersRepository.GetCompanyUsersByEmails(entityId, emails);
                    result = companyUsers.Select(x => x.User.Email).ToList();
                    break;
                case InviteEntityType.Team:
                    var teamUsers = await _teamsUsersRepository.GetTeamUsersByEmails(entityId, emails);
                    result = teamUsers.Select(x => x.User.Email).ToList();
                    break;
                case InviteEntityType.Project:
                    var projectUsers = await _projectsUsersRepository.GetProjectUsersByEmails(entityId, emails);
                    result = projectUsers.Select(x => x.User.Email).ToList();
                    break;
                default:
                    result = new List<string>();
                    break;
            }

            return result;
        }
    }
}