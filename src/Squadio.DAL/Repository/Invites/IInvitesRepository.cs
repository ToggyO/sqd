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
        Task<InviteModel> ActivateInvite(string code);
        Task ActivateInvites(Guid entityId, string email);
        Task DeleteInvites(IEnumerable<Guid> ids);
        // Task<InviteModel> ActivateInvite(Guid inviteId);
        Task<IEnumerable<InviteModel>> GetInvites(
            Guid? entityId = null, 
            Guid? authorId = null, 
            string email = null, 
            InviteEntityType? entityType = null);
        // Task ActivateInvites(Guid entityId, IEnumerable<string> emails);
    }
}