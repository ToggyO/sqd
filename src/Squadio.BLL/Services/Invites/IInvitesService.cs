using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Enums;
using Squadio.DTO.Invites;

namespace Squadio.BLL.Services.Invites
{
    public interface IInvitesService
    {
        Task<Response> InviteToCompany(Guid companyId, Guid authorId, CreateInvitesDTO dto, bool sendInvites = true);
        Task<Response> InviteToTeam(Guid teamId, Guid authorId, CreateInvitesDTO dto, bool sendInvites = true);
        Task<Response> InviteToProject(Guid projectId, Guid authorId, CreateInvitesDTO dto, bool sendInvites = true);
        Task<Response> CancelInvite(Guid entityId, Guid authorId, CancelInvitesDTO dto, EntityType entityType);
        Task<Response> AcceptInvite(Guid userId, string code, EntityType entityType);
        Task<Response> SendSignUpInvites(Guid userId);
    }
}