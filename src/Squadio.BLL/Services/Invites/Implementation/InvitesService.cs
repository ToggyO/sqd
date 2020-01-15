using System;
using System.Linq;
using System.Threading.Tasks;
using Squadio.BLL.Services.Rabbit;
using Squadio.Common.Models.Email;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Companies;
using Squadio.DAL.Repository.Invites;
using Squadio.DAL.Repository.Projects;
using Squadio.DAL.Repository.Teams;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Invites;
using Squadio.DTO.Invites;

namespace Squadio.BLL.Services.Invites.Implementation
{
    public class InvitesService : IInvitesService
    {
        private readonly ICompaniesRepository _companiesRepository;
        private readonly ITeamsRepository _teamsRepository;
        private readonly IProjectsRepository _projectsRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IRabbitService _rabbitService;
        private readonly IInvitesRepository _repository;

        public InvitesService(ICompaniesRepository companiesRepository
            , ITeamsRepository teamsRepository
            , IProjectsRepository projectsRepository
            , IUsersRepository usersRepository
            , IRabbitService rabbitService
            , IInvitesRepository repository)
        {
            _companiesRepository = companiesRepository;
            _teamsRepository = teamsRepository;
            _projectsRepository = projectsRepository;
            _usersRepository = usersRepository;
            _rabbitService = rabbitService;
            _repository = repository;
        }

        public async Task<Response> CreateInvite(string email, string code, Guid entityId, InviteEntityType inviteEntityType, Guid authorId)
        {
            var user = await _usersRepository.GetByEmail(email);
            if (user.Status != UserStatus.Pending)
            {
                return new ErrorResponse();
            }
            
            var invite = new InviteModel
            {
                Email = email,
                IsActivated = false,
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

        public async Task<Response> ActivateInvites(string email)
        {
            var user = await _usersRepository.GetByEmail(email);
            
            var invites = await _repository.GetInvites(email: email, activated: false);
            foreach (var invite in invites)
            {
                await _repository.ActivateInvite(invite.Id);
            }
            
            if (user.Status == UserStatus.Pending)
            {
                user.Status = UserStatus.Active;
                await _usersRepository.Update(user);
            }
            
            return new Response();
        }

        public async Task<Response> SendInvite(string email)
        {
            var user = await _usersRepository.GetByEmail(email);
            if (user.Status != UserStatus.Pending)
            {
                return new ErrorResponse();
            }
            
            var invites = (await _repository.GetInvites(email: email, isSent: false)).ToList();

            var projectInvite = invites.FirstOrDefault(x => x.EntityType == InviteEntityType.Project);
            if (projectInvite != null)
            {
                var project = await _projectsRepository.GetById(projectInvite.EntityId);
                await _rabbitService.Send(new InviteUserEmailModel
                {
                    To = email,
                    AuthorName = projectInvite.Creator.Name,
                    EntityName = project.Name,
                    InviteEntityType = InviteEntityType.Project
                });
                return new Response();
            }

            var teamInvite = invites.FirstOrDefault(x => x.EntityType == InviteEntityType.Team);
            if (teamInvite != null)
            {
                var team = await _teamsRepository.GetById(teamInvite.EntityId);
                await _rabbitService.Send(new InviteUserEmailModel
                {
                    To = email,
                    AuthorName = teamInvite.Creator.Name,
                    EntityName = team.Name,
                    InviteEntityType = InviteEntityType.Team
                });
                return new Response();
            }

            var companyInvite = invites.FirstOrDefault(x => x.EntityType == InviteEntityType.Company);
            if (companyInvite != null)
            {
                var team = await _teamsRepository.GetById(companyInvite.EntityId);
                await _rabbitService.Send(new InviteUserEmailModel
                {
                    To = email,
                    AuthorName = companyInvite.Creator.Name,
                    EntityName = team.Name,
                    InviteEntityType = InviteEntityType.Company
                });
                return new Response();
            }
            
            return new ErrorResponse();
        }
    }
}