using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Models.Invites;
using Squadio.DTO.Models.Teams;
using Squadio.DTO.Models.Users;

namespace Squadio.API.Handlers.Teams
{
    public interface ITeamsHandler
    {
        Task<Response<PageModel<UserWithRoleDTO>>> GetTeamUsers(Guid teamId, PageModel model);
        Task<Response<PageModel<TeamDTO>>> GetTeams(PageModel model, Guid? companyId = null);
        Task<Response<TeamDTO>> GetById(Guid id);
        Task<Response<TeamDTO>> Create(Guid companyId, TeamCreateDTO dto, ClaimsPrincipal claims);
        Task<Response<TeamDTO>> Update(Guid teamId, TeamUpdateDTO dto, ClaimsPrincipal claims);
        Task<Response<TeamDTO>> Delete(Guid teamId, ClaimsPrincipal claims);
        Task<Response> DeleteTeamUser(Guid teamId, Guid userId, ClaimsPrincipal claims);
        Task<Response> LeaveTeam(Guid teamId, ClaimsPrincipal claims);
        Task<Response> InviteTeamUsers(Guid teamId, CreateInvitesDTO dto, ClaimsPrincipal claims);
    }
}