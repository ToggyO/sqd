using System;
using System.Threading.Tasks;
using Squadio.DTO.Teams;

namespace Squadio.BLL.Services.Teams
{
    public interface ITeamsService
    {
        Task<TeamDTO> Create(Guid userId, CreateTeamDTO dto);
    }
}