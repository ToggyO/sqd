using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Invites;

namespace Squadio.DAL.Repository.Invites
{
    public interface IInvitesRepository
    {
        Task<InviteModel> CreateInvite(InviteModel entity);
        Task<InviteModel> GetInviteByCode(string code);
        Task<InviteModel> ActivateInvite(Guid inviteId);
        Task DeleteInvites(IEnumerable<Guid> ids);
        Task<IEnumerable<InviteModel>> GetInvites(Guid? entityId = null, 
            Guid? authorId = null, 
            InviteEntityType? entityType = null, 
            bool? activated = null,
            bool? isSent = null);
        Task ActivateInvites(Guid entityId, IEnumerable<string> emails);
        Task<InviteModel> ActivateInvite(string code);
    }
}