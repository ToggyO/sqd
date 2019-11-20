using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Teams;

namespace Squadio.BLL.Services.Teams
{
    public interface ITeamsService
    {
        Task<Response<TeamDTO>> Create(Guid userId, Guid companyId, TeamCreateDTO dto);
        Task<Response<TeamDTO>> Update(Guid teamId, Guid userId, TeamUpdateDTO dto);
        Task<Response> DeleteUserFromTeam(Guid teamId, Guid removeUserId, Guid currentUserId);
        Task<Response> LeaveTeam(Guid teamId, Guid userId);
        Task<Response> DeleteUserFromTeamsByCompanyId(Guid companyId, Guid removeUserId);
    }
}