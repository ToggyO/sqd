using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Invites;
using Squadio.DTO.Invites;

namespace Squadio.BLL.Providers.Invites
{
    public interface IInvitesProvider
    {
        Task<Response<InviteModel>> GetInviteByCode(string code);
        Task<Response<IEnumerable<InviteDTO>>> GetInvitesByEntityId(Guid entityId, Guid userId, EntityType entityType, bool? activated = null);
        Task<Response<IEnumerable<InviteDTO>>> GetInvitesByEntityId(Guid entityId, bool? activated = null);
    }
}