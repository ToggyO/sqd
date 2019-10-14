using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Teams;

namespace Squadio.API.Handlers.Teams
{
    public interface ITeamsHandler
    {
        Task<Response<TeamDTO>> GetById(Guid id);
        Task<Response<TeamDTO>> Create(CreateTeamDTO dto, ClaimsPrincipal claims);
    }
}