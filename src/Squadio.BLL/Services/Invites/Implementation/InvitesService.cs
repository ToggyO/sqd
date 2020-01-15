using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Companies;
using Squadio.DAL.Repository.Invites;
using Squadio.DAL.Repository.Projects;
using Squadio.DAL.Repository.Teams;
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
        private readonly IInvitesRepository _repository;

        public InvitesService(ICompaniesRepository companiesRepository
            , ITeamsRepository teamsRepository
            , IProjectsRepository projectsRepository
            , IInvitesRepository repository)
        {
            _companiesRepository = companiesRepository;
            _teamsRepository = teamsRepository;
            _projectsRepository = projectsRepository;
            _repository = repository;
        }
        
        public Task<Response> CreateInvite(string email, string code, Guid entityId, InviteEntityType inviteEntityType, Guid authorId)
        {
            throw new NotImplementedException();
        }

        public Task<Response> ActivateInvites(string email)
        {
            throw new NotImplementedException();
        }

        public Task<Response> SendInvite(string email)
        {
            throw new NotImplementedException();
        }
    }
}