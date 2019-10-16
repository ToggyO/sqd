using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Pages;
using Squadio.DTO.Teams;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.Teams
{
    public interface ITeamsHandler
    {
        Task<Response<PageModel<UserDTO>>> GetTeamUsers(Guid teamId, PageModel model);
        Task<Response<PageModel<TeamDTO>>> GetTeams(PageModel model);
        Task<Response<TeamDTO>> GetById(Guid id);
        Task<Response<TeamDTO>> Create(CreateTeamDTO dto, ClaimsPrincipal claims);
    }
}