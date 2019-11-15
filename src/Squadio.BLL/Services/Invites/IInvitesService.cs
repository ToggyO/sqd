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
        Task<Response<IEnumerable<InviteDTO>>> InviteToCompany(Guid companyId, Guid authorId, CreateInvitesDTO dto);
        Task<Response<IEnumerable<InviteDTO>>> InviteToTeam(Guid teamId, Guid authorId, CreateInvitesDTO dto);
        Task<Response<IEnumerable<InviteDTO>>> InviteToProject(Guid projectId, Guid authorId, CreateInvitesDTO dto);
        Task<Response> CancelInvite(Guid entityId, Guid authorId, CancelInvitesDTO dto, EntityType entityType);
        Task<Response> AcceptInvite(Guid userId, string code);
    }
}