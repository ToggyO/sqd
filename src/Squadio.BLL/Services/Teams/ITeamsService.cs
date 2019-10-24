using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Teams;

namespace Squadio.BLL.Services.Teams
{
    public interface ITeamsService
    {
        Task<Response<TeamDTO>> Create(Guid userId, Guid companyId, CreateTeamDTO dto);
    }
}