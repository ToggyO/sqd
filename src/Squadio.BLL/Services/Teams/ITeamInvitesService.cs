using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Invites;

namespace Squadio.BLL.Services.Teams
{
    public interface ITeamInvitesService
    {
        Task<Response> CreateInvite(Guid teamId, Guid authorId, CreateInvitesDTO dto, bool sendInvites = true);
        Task<Response> CancelInvite(Guid teamId, Guid authorId, CancelInvitesDTO dto);
        Task<Response> AcceptInvite(Guid userId, string code);
        Task<Response> AcceptInvite(Guid teamId, Guid userId);
    }
}