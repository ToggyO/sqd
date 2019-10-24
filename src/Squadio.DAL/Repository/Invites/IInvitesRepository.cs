using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squadio.Domain.Models.Invites;

namespace Squadio.DAL.Repository.Invites
{
    public interface IInvitesRepository
    {
        Task<InviteModel> CreateTeamInvite(string email, Guid teamId);
        Task<InviteModel> CreateProjectInvite(string email, Guid projectId);
        Task<InviteModel> GetInviteByCode(string code);
        Task<InviteModel> ActivateInvite(Guid inviteId);
        Task<IEnumerable<InviteModel>> GetTeamInvites(Guid teamId);
        Task<IEnumerable<InviteModel>> GetProjectInvites(Guid projectId);
    }
}