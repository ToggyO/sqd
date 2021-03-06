using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.BLL.Providers.Teams;
using Squadio.BLL.Services.Membership;
using Squadio.BLL.Services.Teams;
using Squadio.Common.Extensions;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Models.Invites;
using Squadio.DTO.Models.Teams;
using Squadio.DTO.Models.Users;

namespace Squadio.API.Handlers.Teams.Implementations
{
    public class TeamsHandler : ITeamsHandler
    {
        private readonly ITeamsProvider _provider;
        private readonly ITeamsService _service;
        private readonly IMembershipService _membershipService;
        public TeamsHandler(ITeamsProvider provider
            , ITeamsService service
            , IMembershipService membershipService)
        {
            _provider = provider;
            _service = service;
            _membershipService = membershipService;
        }

        public async Task<Response<PageModel<UserWithRoleDTO>>> GetTeamUsers(Guid teamId, PageModel model)
        {
            var result = await _provider.GetTeamUsers(teamId, model);
            return result;
        }

        public async Task<Response<PageModel<TeamDTO>>> GetTeams(PageModel model, Guid? companyId = null)
        {
            var result = await _provider.GetTeams(model, companyId);
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

        public async Task<Response<TeamDTO>> Delete(Guid teamId, ClaimsPrincipal claims)
        {
            var result = await _service.Delete(teamId, claims.GetUserId());
            return result;
        }

        public async Task<Response> DeleteTeamUser(Guid teamId, Guid userId, ClaimsPrincipal claims)
        {
            var result = await _membershipService.DeleteUserFromTeam(teamId, userId, claims.GetUserId());
            return result;
        }

        public async Task<Response> LeaveTeam(Guid teamId, ClaimsPrincipal claims)
        {
            var result = await _membershipService.DeleteUserFromTeam(teamId, claims.GetUserId(), claims.GetUserId(), false);
            return result;
        }

        public async Task<Response> InviteTeamUsers(Guid teamId, CreateInvitesDTO dto, ClaimsPrincipal claims)
        {
            //TODO:
            throw new NotImplementedException();
            // var result = await _membershipService.InviteUsersToTeam(teamId, claims.GetUserId(), dto);
            // return result;
        }
    }
}