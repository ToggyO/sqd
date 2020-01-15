using System;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Enums;
using Squadio.DTO.Invites;

namespace Squadio.BLL.Services.Invites.Implementation
{
    public class InvitesService : IInvitesService
    {
        public Response<InviteDTO> CreateInvite(string email, string code, Guid entityId, EntityType entityType, Guid authorId)
        {
            throw new NotImplementedException();
        }
    }
}