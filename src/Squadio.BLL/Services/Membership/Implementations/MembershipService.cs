using System;
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

        //TODO: think how optimize invites. Now here many access to DB!
        //TODO: add sending notification about adding user
        #region Invite

        public async Task<Response> InviteUsersToCompany(Guid companyId, Guid authorId, CreateInvitesDTO dto, bool sendMails = true)
        {
            var company = await _companiesRepository.GetById(companyId);
            
            var distincted = dto.Emails.Distinct().ToList();
            
            var requestedUser = await _companiesUsersRepository.GetCompanyUser(companyId, authorId);
            
            var companyUsers = await _companiesUsersRepository.GetCompanyUsersByEmails(new PageModel {Page = 1, PageSize = 1000000}
                , companyId
                , distincted);
            
            var existedEmails = companyUsers.Items.Select(x => x.User.Email).ToList();
            var notExistedEmails = distincted.Except(existedEmails).ToList();
            
            foreach (var email in notExistedEmails)
            {
                var code = CodeHelper.GenerateCodeAsGuid();
                
                var user = (await _usersProvider.GetByEmail(email)).Data;
                if (user == null)
                {
                    var createUserDTO = new UserCreateDTO()
                    {
                        Email = email,
                        Step = RegistrationStep.New,
                        MembershipStatus = MembershipStatus.Member,
                        UserStatus = UserStatus.Pending
                    };
                    user = (await _usersService.CreateUserWithPasswordRestore(createUserDTO, code)).Data;
                }

                await _companiesUsersRepository.AddCompanyUser(companyId, user.Id, MembershipStatus.Member);

                if (user.Status == UserStatus.Pending)
                {
                    await _invitesService.CreateInvite(email, code, companyId, InviteEntityType.Company, authorId);
                }
                else if (sendMails)
                {
                    //TODO: some way to send invite
                    // await _rabbitService.Send(new AddUserEmailModel
                    // {
                    //     To = email,
                    //     AuthorName = requestedUser.User.Name,
                    //     EntityName = company.Name,
                    //     EntityType = EntityType.Company
                    // });
                }

                if (sendMails)
                {
                    await _invitesService.SendInvite(email);
                }
            }
            
            return new Response();
        }
        public async Task<Response> InviteUsersToTeam(Guid teamId, Guid authorId, CreateInvitesDTO dto, bool sendMails = true)
        {
            var team = await _teamsRepository.GetById(teamId);
            var companyId = team.CompanyId;
            
            await InviteUsersToCompany(companyId, authorId, dto, false);
            var distincted = dto.Emails.Distinct().ToList();
            
            var requestedUser = await _companiesUsersRepository.GetCompanyUser(companyId, authorId);
            
            var teamUsers = await _teamsUsersRepository.GetTeamUsersByEmails(new PageModel {Page = 1, PageSize = 1000000}
                , teamId
                , distincted);
            
            var existedEmails = teamUsers.Items.Select(x => x.User.Email).ToList();
            var notExistedEmails = distincted.Except(existedEmails).ToList();
            
            foreach (var email in notExistedEmails)
            {
                var code = CodeHelper.GenerateCodeAsGuid();
                
                var user = (await _usersProvider.GetByEmail(email)).Data;

                await _teamsUsersRepository.AddTeamUser(teamId, user.Id, MembershipStatus.Member);
                
                if (user.Status == UserStatus.Pending)
                {
                    await _invitesService.CreateInvite(email, code, teamId, InviteEntityType.Team, authorId);
                }
                else if (sendMails)
                {
                    //TODO: some way to send invite
                    // await _rabbitService.Send(new AddUserEmailModel
                    // {
                    //     To = email,
                    //     AuthorName = requestedUser.User.Name,
                    //     EntityName = team.Name,
                    //     EntityType = EntityType.Team
                    // });
                }

                if (sendMails)
                {
                    await _invitesService.SendInvite(email);
                }
            }
            
            return new Response();
        }

        public async Task<Response> InviteUsersToProject(Guid projectId, Guid authorId, CreateInvitesDTO dto, bool sendMails = true)
        {
            var project = await _projectsRepository.GetById(projectId);
            var teamId = project.TeamId;
            var companyId = project.Team.CompanyId;
            
            await InviteUsersToTeam(teamId, authorId, dto, false);
            var distincted = dto.Emails.Distinct().ToList();
            
            var requestedUser = await _companiesUsersRepository.GetCompanyUser(companyId, authorId);
            
            var teamUsers = await _projectsUsersRepository.GetProjectUsersByEmails(new PageModel {Page = 1, PageSize = 1000000}
                , teamId
                , distincted);
            
            var existedEmails = teamUsers.Items.Select(x => x.User.Email).ToList();
            var notExistedEmails = distincted.Except(existedEmails).ToList();
            
            foreach (var email in notExistedEmails)
            {
                var code = CodeHelper.GenerateCodeAsGuid();
                
                var user = (await _usersProvider.GetByEmail(email)).Data;

                await _projectsUsersRepository.AddProjectUser(projectId, user.Id, MembershipStatus.Member);
                
                if (user.Status == UserStatus.Pending)
                {
                    await _invitesService.CreateInvite(email, code, projectId, InviteEntityType.Project, authorId);
                }
                else if (sendMails)
                {
                    //TODO: some way to send invite
                    // await _rabbitService.Send(new AddUserEmailModel
                    // {
                    //     To = email,
                    //     AuthorName = requestedUser.User.Name,
                    //     EntityName = project.Name,
                    //     EntityType = EntityType.Project
                    // });
                }

                if (sendMails)
                {
                    await _invitesService.SendInvite(email);
                }
            }
            
            return new Response();
        }
        
        #endregion
        
        #region Delete

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
        
        #endregion
    }
}