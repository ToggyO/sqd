using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Teams;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.Teams
{
    public interface ITeamsHandler
    {
        Task<Response<PageModel<TeamUserDTO>>> GetTeamUsers(Guid teamId, PageModel model);
        Task<Response<PageModel<TeamDTO>>> GetTeams(PageModel model);
        Task<Response<TeamDTO>> GetById(Guid id);
        Task<Response<TeamDTO>> Create(Guid companyId, TeamCreateDTO dto, ClaimsPrincipal claims);
        Task<Response<TeamDTO>> Update(Guid teamId, TeamUpdateDTO dto, ClaimsPrincipal claims);
    }
}