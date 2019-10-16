using System;
using System.Net;
using System.Threading.Tasks;
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

namespace Squadio.BLL.Services.Invites.Implementation
{
    public class InvitesService : IInvitesService
    {
        private readonly IInvitesRepository _repository;
        private readonly IEmailService<InviteToTeamEmailModel> _inviteToTeamMailService;
        private readonly IEmailService<InviteToProjectEmailModel> _inviteToProjectMailService;
        private readonly IUsersRepository _usersRepository;
        private readonly ICompaniesRepository _companiesRepository;
        private readonly ICompaniesUsersRepository _companiesUsersRepository;
        private readonly ITeamsRepository _teamsRepository;
        private readonly ITeamsUsersRepository _teamsUsersRepository;
        private readonly IProjectsUsersRepository _projectsUsersRepository;
        public InvitesService(IInvitesRepository repository
            , IEmailService<InviteToTeamEmailModel> inviteToTeamMailService
            , IEmailService<InviteToProjectEmailModel> inviteToProjectMailService
            , IUsersRepository usersRepository
            , ICompaniesRepository companiesRepository
            , ICompaniesUsersRepository companiesUsersRepository
            , ITeamsRepository teamsRepository
            , ITeamsUsersRepository teamsUsersRepository
            , IProjectsUsersRepository projectsUsersRepository)
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
        }

        public async Task<Response> InviteToTeam(string authorName, string teamName, Guid teamId, string email)
        {
            var invite = await _repository.CreateInvite(email);

            await _inviteToTeamMailService.Send(new InviteToTeamEmailModel
            {
                To = email,
                AuthorName = authorName,
                Code = invite.Code,
                TeamId = teamId.ToString(),
                TeamName = teamName
            });

            return new Response();
        }

        public async Task<Response> InviteToProject(string authorName, string projectName, Guid projectId, string email)
        {
            var invite = await _repository.CreateInvite(email);

            await _inviteToProjectMailService.Send(new InviteToProjectEmailModel
            {
                To = email,
                AuthorName = authorName,
                Code = invite.Code,
                ProjectId = projectId.ToString(),
                ProjectName = projectName
            });

            return new Response();
        }

        public async Task<Response> AcceptInviteToTeam(Guid userId, Guid teamId, string code)
        {
            var invite = await _repository.GetInviteByCode(code);
            
            if (invite == null || invite?.Activated == true)
            {
                return new ErrorResponse
                {
                    Code = ErrorCodes.Security.InviteInvalid,
                    Message = ErrorMessages.Security.InviteInvalid,
                    HttpStatusCode = HttpStatusCode.BadRequest
                };
            }

            var teamUser = await _teamsUsersRepository.GetTeamUser(teamId, userId);
            if(teamUser != null)
                return new Response();
            
            var team = await _teamsRepository.GetById(teamId);
            if (team == null)
            {
                return new ErrorResponse
                {
                    Code = ErrorCodes.Common.NotFound,
                    Message = ErrorMessages.Common.NotFound,
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Errors = new []
                    {
                        new Error
                        {
                            Code = ErrorCodes.Common.NotFound,
                            Message = ErrorMessages.Common.NotFound,
                            Field = ErrorFields.Team.Id
                        }
                    }
                };
            }

            var companyUser = await _companiesUsersRepository.GetCompanyUser(team.CompanyId, userId);
            if (companyUser == null)
            {
                var company = await _companiesRepository.GetById(team.CompanyId);
                if (company == null)
                {
                    return new ErrorResponse
                    {
                        Code = ErrorCodes.Common.NotFound,
                        Message = ErrorMessages.Common.NotFound,
                        HttpStatusCode = HttpStatusCode.BadRequest,
                        Errors = new[]
                        {
                            new Error
                            {
                                Code = ErrorCodes.Common.NotFound,
                                Message = ErrorMessages.Common.NotFound,
                                Field = ErrorFields.Company.Id
                            }
                        }
                    };
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
                return new ErrorResponse
                {
                    Code = ErrorCodes.Security.InviteInvalid,
                    Message = ErrorMessages.Security.InviteInvalid,
                    HttpStatusCode = HttpStatusCode.BadRequest
                };
            }

            var projectUser = await _projectsUsersRepository.GetProjectUser(projectId, userId);
            if(projectUser != null)
                return new Response();
            
            await _projectsUsersRepository.AddProjectUser(projectId, userId, UserStatus.Member);
            await _repository.ActivateInvite(invite.Id);
            
            return new Response();
        }
    }
}