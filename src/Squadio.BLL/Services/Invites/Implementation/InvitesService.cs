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
        private readonly ICompaniesUsersRepository _companiesUsersRepository;
        private readonly ITeamsUsersRepository _teamsUsersRepository;
        private readonly IProjectsUsersRepository _projectsUsersRepository;
        private readonly ILogger<InvitesService> _logger;
        
        public InvitesService(IInvitesRepository repository
            , IRabbitService rabbitService
            , ICompaniesUsersRepository companiesUsersRepository
            , ITeamsUsersRepository teamsUsersRepository
            , IProjectsUsersRepository projectsUsersRepository
            , ILogger<InvitesService> logger)
        {
            _repository = repository;
            _rabbitService = rabbitService;
            _companiesUsersRepository = companiesUsersRepository;
            _teamsUsersRepository = teamsUsersRepository;
            _projectsUsersRepository = projectsUsersRepository;
            _logger = logger;
        }
        
        #endregion

        public async Task<Response> SendSignUpInvites(Guid userId)
        {
            var pageModel = new PageModel {Page = 1, PageSize = 1};

            var userCompany = (await _companiesUsersRepository.GetCompaniesUsers(pageModel, userId))
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

            var userTeam = (await _teamsUsersRepository.GetTeamsUsers(pageModel, userId, userCompany.CompanyId))
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
                (await _projectsUsersRepository.GetProjectsUsers(pageModel, 
                    teamId: userTeam.TeamId, 
                    userId: userId))
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

    }
}