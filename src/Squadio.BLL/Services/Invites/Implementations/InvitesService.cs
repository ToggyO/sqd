﻿﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Squadio.BLL.Services.Notifications.Emails;
using Squadio.Common.Helpers;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Companies;
using Squadio.DAL.Repository.Invites;
using Squadio.DAL.Repository.Projects;
using Squadio.DAL.Repository.Teams;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Invites;

namespace Squadio.BLL.Services.Invites.Implementations
{
    public class InvitesService : IInvitesService
    {
        private readonly ICompaniesRepository _companiesRepository;
        private readonly ITeamsRepository _teamsRepository;
        private readonly IProjectsRepository _projectsRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IInvitesRepository _repository;
        private readonly IEmailNotificationsService _emailNotificationsService;

        public InvitesService(ICompaniesRepository companiesRepository
            , ITeamsRepository teamsRepository
            , IProjectsRepository projectsRepository
            , IUsersRepository usersRepository
            , IInvitesRepository repository
            , IEmailNotificationsService emailNotificationsService)
        {
            _companiesRepository = companiesRepository;
            _teamsRepository = teamsRepository;
            _projectsRepository = projectsRepository;
            _usersRepository = usersRepository;
            _repository = repository;
            _emailNotificationsService = emailNotificationsService;
        }

        public async Task<Response> GetInviteByCode(string code)
        {
            throw new NotImplementedException();
        }

        public async Task<Response> CreateInvite(string email, Guid entityId, InviteEntityType inviteEntityType, Guid authorId, string code = null)
        {
            if (string.IsNullOrEmpty(code))
            {
                code = CodeHelper.GenerateCodeAsGuid();
            }
            
            var invite = new InviteModel
            {
                Email = email,
                IsDeleted = false,
                IsSent = false,
                CreatedDate = DateTime.UtcNow,
                Code = code,
                EntityId = entityId,
                EntityType = inviteEntityType,
                CreatorId = authorId
            };
            
            await _repository.CreateInvite(invite);
            
            return new Response();
        }

        public async Task<Response> SendInvite(string email)
        {
            // var user = await _usersRepository.GetByEmail(email);
            // if (user.Status != UserStatus.Pending)
            // {
            //     return new ErrorResponse();
            // }
            //
            // var invites = (await _repository.GetInvites(email: email, isSent: false)).ToList();
            //
            // var projectInvite = invites.FirstOrDefault(x => x.EntityType == InviteEntityType.Project);
            // if (projectInvite != null)
            // {
            //     var project = await _projectsRepository.GetById(projectInvite.EntityId);
            //     //TODO: some way to send invite
            //     // await _rabbitService.Send(new InviteUserEmailModel
            //     // {
            //     //     To = email,
            //     //     AuthorName = projectInvite.Creator.Name,
            //     //     EntityName = project.Name,
            //     //     EntityType = InviteEntityType.Project,
            //     //     Code = projectInvite.Code
            //     // });
            //     return new Response();
            // }
            //
            // var teamInvite = invites.FirstOrDefault(x => x.EntityType == InviteEntityType.Team);
            // if (teamInvite != null)
            // {
            //     var team = await _teamsRepository.GetById(teamInvite.EntityId);
            //     //TODO: some way to send invite
            //     // await _rabbitService.Send(new InviteUserEmailModel
            //     // {
            //     //     To = email,
            //     //     AuthorName = teamInvite.Creator.Name,
            //     //     EntityName = team.Name,
            //     //     EntityType = InviteEntityType.Team,
            //     //     Code = teamInvite.Code
            //     // });
            //     return new Response();
            // }
            //
            // var companyInvite = invites.FirstOrDefault(x => x.EntityType == InviteEntityType.Company);
            // if (companyInvite != null)
            // {
            //     var team = await _teamsRepository.GetById(companyInvite.EntityId);
            //     //TODO: some way to send invite
            //     // await _rabbitService.Send(new InviteUserEmailModel
            //     // {
            //     //     To = email,
            //     //     AuthorName = companyInvite.Creator.Name,
            //     //     EntityName = team.Name,
            //     //     EntityType = InviteEntityType.Company,
            //     //     Code = companyInvite.Code
            //     // });
            //     return new Response();
            // }
            //
            // return new ErrorResponse();
            throw new NotImplementedException();
        }

        public async Task<Response> RemoveInvites(string email, Guid entityId, InviteEntityType inviteEntityType)
        {
            var invites = await _repository.GetInvites(email: email, entityId: entityId, entityType: inviteEntityType);
            await _repository.DeleteInvites(invites.Select(x => x.Id));
            return new Response();
        }
    }
}