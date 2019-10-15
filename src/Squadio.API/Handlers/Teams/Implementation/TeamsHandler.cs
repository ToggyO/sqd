using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.BLL.Providers.Teams;
using Squadio.BLL.Services.Teams;
using Squadio.Common.Extensions;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Teams;

namespace Squadio.API.Handlers.Teams.Implementation
{
    public class TeamsHandler : ITeamsHandler
    {
        private readonly ITeamsProvider _provider;
        private readonly ITeamsService _service;
        public TeamsHandler(ITeamsProvider provider
            , ITeamsService service)
        {
            _provider = provider;
            _service = service;
        }

        public async Task<Response<TeamDTO>> GetById(Guid id)
        {
            var result = await _provider.GetById(id);
            return result;
        }

        public async Task<Response<TeamDTO>> Create(CreateTeamDTO dto, ClaimsPrincipal claims)
        {
            var result = await _service.Create(claims.GetUserId(), dto);
            return result;
        }
    }
}