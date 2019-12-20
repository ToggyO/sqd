using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Teams;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.Teams
{
    public interface ITeamsHandler
    {
        Task<Response<PageModel<UserWithRoleDTO>>> GetTeamUsers(Guid teamId, PageModel model);
        Task<Response<PageModel<TeamDTO>>> GetTeams(PageModel model, TeamFilter filter);
        Task<Response<TeamDTO>> GetById(Guid id);
        Task<Response<TeamDTO>> Create(Guid companyId, TeamCreateDTO dto, ClaimsPrincipal claims);
        Task<Response<TeamDTO>> Update(Guid teamId, TeamUpdateDTO dto, ClaimsPrincipal claims);
        Task<Response> DeleteTeamUser(Guid teamId, Guid userId, ClaimsPrincipal claims);
        Task<Response> LeaveTeam(Guid teamId, ClaimsPrincipal claims);
    }
}