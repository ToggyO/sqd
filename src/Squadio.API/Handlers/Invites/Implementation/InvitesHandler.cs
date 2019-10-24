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
            var result = await _service.InviteToTeam(teamId, claims.GetUserId(), dto);
            return result;
        }

        public async Task<Response<IEnumerable<InviteDTO>>> InviteToProject(Guid projectId, CreateInvitesDTO dto, ClaimsPrincipal claims)
        {
            var result = await _service.InviteToProject(projectId, claims.GetUserId(), dto);
            return result;
        }

        public async Task<Response> AcceptInviteToTeam(Guid teamId, ClaimsPrincipal claims, string code)
        {
            var result = await _service.AcceptInviteToTeam(claims.GetUserId(), teamId, code);
            return result;
        }

        public async Task<Response> AcceptInviteToProject(Guid projectId, ClaimsPrincipal claims, string code)
        {
            var result = await _service.AcceptInviteToProject(claims.GetUserId(), projectId, code);
            return result;
        }
    }
}