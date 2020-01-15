using System;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Enums;
using Squadio.DTO.Invites;

namespace Squadio.BLL.Services.Invites
{
    public interface IInvitesService
    {
        Response<InviteDTO> CreateInvite(string email, string code, Guid entityId, EntityType entityType, Guid authorId);
        Response ActivateInvite(string code);
        
        Response SendCompanyInvites(Guid companyId);
        Response SendTeamInvites(Guid teamId);
        Response SendProjectInvites(Guid projectId);
    }
}