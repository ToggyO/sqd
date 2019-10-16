using System;
using System.Threading.Tasks;
using Squadio.Domain.Models.Invites;

namespace Squadio.DAL.Repository.Invites
{
    public interface IInvitesRepository
    {
        Task<InviteModel> CreateInvite(string email);
        Task<InviteModel> GetInviteByCode(string code);
        Task<InviteModel> ActivateInvite(Guid inviteId);
    }
}