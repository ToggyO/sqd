using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.BLL.Providers.Invites;
using Squadio.BLL.Services.Invites;
using Squadio.Common.Extensions;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Invites;

namespace Squadio.API.Handlers.Invites.Implementation
{
    public class InvitesHandler : IInvitesHandler
    {
        private readonly IInvitesService _service;
        private readonly IInvitesProvider _provider;

        public InvitesHandler(IInvitesService service
            , IInvitesProvider provider)
        {
            _service = service;
            _provider = provider;
        }
        
        public async Task<Response<IEnumerable<InviteDTO>>> InviteToTeam(Guid teamId, CreateInvitesDTO dto, ClaimsPrincipal claims)
        {
            var userId = claims.GetUserId();
            if (!userId.HasValue)
            {
                return claims.Unauthorized<IEnumerable<InviteDTO>>();
            }
            var result = await _service.InviteToTeam(teamId, userId.Value, dto);
            return result;
        }

        public async Task<Response<IEnumerable<InviteDTO>>> InviteToProject(Guid projectId, CreateInvitesDTO dto, ClaimsPrincipal claims)
        {
            var userId = claims.GetUserId();
            if (!userId.HasValue)
            {
                return claims.Unauthorized<IEnumerable<InviteDTO>>();
            }
            var result = await _service.InviteToProject(projectId, userId.Value, dto);
            return result;
        }

        public async Task<Response> AcceptInviteToTeam(Guid teamId, ClaimsPrincipal claims, string code)
        {
            var userId = claims.GetUserId();
            if (!userId.HasValue)
            {
                return claims.Unauthorized();
            }
            var result = await _service.AcceptInviteToTeam(userId.Value, teamId, code);
            return result;
        }

        public async Task<Response> AcceptInviteToProject(Guid projectId, ClaimsPrincipal claims, string code)
        {
            var userId = claims.GetUserId();
            if (!userId.HasValue)
            {
                return claims.Unauthorized();
            }
            var result = await _service.AcceptInviteToProject(userId.Value, projectId, code);
            return result;
        }
    }
}