using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Models.Teams;
using Squadio.DTO.Teams;

namespace Squadio.BLL.Services.Teams
{
    public interface ITeamsService
    {
        Task<Response<TeamDTO>> Create(Guid userId, Guid companyId, TeamCreateDTO dto, bool sendInvites = true);
        Task<Response<TeamDTO>> Update(Guid teamId, Guid userId, TeamUpdateDTO dto);
        Task<Response<TeamDTO>> Delete(Guid teamId, Guid userId);
        //Task<Response> InviteUsers(Guid teamId, Guid authorId, CreateInvitesDTO dto, bool sendMails = true);
        //Task<Response> DeleteUserFromTeam(Guid teamId, Guid removeUserId, Guid currentUserId);
        //Task<Response> LeaveTeam(Guid teamId, Guid userId);
        //Task<Response> DeleteUserFromTeamsByCompanyId(Guid companyId, Guid removeUserId);
    }
}