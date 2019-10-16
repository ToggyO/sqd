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
        
        public Task<Response<IEnumerable<InviteDTO>>> InviteToTeam(Guid teamId, CreateInvitesDTO dto, ClaimsPrincipal claims)
        {
            var result = _service.InviteToTeam(teamId, claims.GetUserId(), dto);
            return result;
        }

        public Task<Response<IEnumerable<InviteDTO>>> InviteToProject(Guid projectId, CreateInvitesDTO dto, ClaimsPrincipal claims)
        {
            var result = _service.InviteToProject(projectId, claims.GetUserId(), dto);
            return result;
        }
    }
}