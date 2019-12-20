using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.BLL.Providers.Teams;
using Squadio.BLL.Services.Teams;
using Squadio.Common.Extensions;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Teams;
using Squadio.DTO.Users;

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

        public async Task<Response<PageModel<UserWithRoleDTO>>> GetTeamUsers(Guid teamId, PageModel model)
        {
            var result = await _provider.GetTeamUsers(teamId, model);
            return result;
        }

        public async Task<Response<PageModel<TeamDTO>>> GetTeams(PageModel model, TeamFilter filter)
        {
            var result = await _provider.GetTeams(model, filter);
            return result;
        }

        public async Task<Response<TeamDTO>> GetById(Guid id)
        {
            var result = await _provider.GetById(id);
            return result;
        }

        public async Task<Response<TeamDTO>> Create(Guid companyId, TeamCreateDTO dto, ClaimsPrincipal claims)
        {
            var result = await _service.Create(claims.GetUserId(), companyId, dto);
            return result;
        }

        public async Task<Response<TeamDTO>> Update(Guid teamId, TeamUpdateDTO dto, ClaimsPrincipal claims)
        {
            var result = await _service.Update(teamId, claims.GetUserId(), dto);
            return result;
        }

        public async Task<Response> DeleteTeamUser(Guid teamId, Guid userId, ClaimsPrincipal claims)
        {
            var result = await _service.DeleteUserFromTeam(teamId, userId, claims.GetUserId());
            return result;
        }

        public async Task<Response> LeaveTeam(Guid teamId, ClaimsPrincipal claims)
        {
            var result = await _service.LeaveTeam(teamId, claims.GetUserId());
            return result;
        }
    }
}