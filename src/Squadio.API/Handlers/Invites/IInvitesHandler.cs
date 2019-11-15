using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Invites;

namespace Squadio.API.Handlers.Invites
{
    public interface IInvitesHandler
    {
        Task<Response<IEnumerable<InviteDTO>>> InviteToCompany(Guid companyId, CreateInvitesDTO dto, ClaimsPrincipal claims);
        Task<Response<IEnumerable<InviteDTO>>> InviteToTeam(Guid teamId, CreateInvitesDTO dto, ClaimsPrincipal claims);
        Task<Response<IEnumerable<InviteDTO>>> InviteToProject(Guid projectId, CreateInvitesDTO dto, ClaimsPrincipal claims);
        Task<Response> AcceptInvite(ClaimsPrincipal claims, string code);
    }
}