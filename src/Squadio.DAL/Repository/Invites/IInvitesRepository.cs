using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squadio.Domain.Models.Invites;

namespace Squadio.DAL.Repository.Invites
{
    public interface IInvitesRepository
    {
        Task<InviteModel> CreateInvite(InviteModel entity);
        Task<InviteModel> GetInviteByCode(string code);
        Task<InviteModel> ActivateInvite(Guid inviteId);
        Task<IEnumerable<InviteModel>> GetInvites(Guid entityId, bool? activated = null);
        Task ActivateInvites(Guid entityId, IEnumerable<string> emails);
        Task<InviteModel> ActivateInvite(string code);
    }
}