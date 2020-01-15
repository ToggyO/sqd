using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Invites;

namespace Squadio.BLL.Services.Invites
{
    public interface IInvitesService
    {
        Task<Response> CreateInvite(string email, string code, Guid entityId, InviteEntityType inviteEntityType, Guid authorId);
        Task<Response> ActivateInvites(string email);
        Task<Response> SendInvite(string email);
    }
}