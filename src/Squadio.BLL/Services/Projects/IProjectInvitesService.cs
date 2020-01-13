using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Invites;

namespace Squadio.BLL.Services.Projects
{
    public interface IProjectInvitesService
    {
        Task<Response> CreateInvite(Guid projectId, Guid authorId, CreateInvitesDTO dto, bool sendInvites = true);
        Task<Response> CancelInvite(Guid projectId, Guid authorId, CancelInvitesDTO dto);
        Task<Response> AcceptInvite(Guid userId, string code);
        Task<Response> AcceptInvite(Guid projectId, Guid userId);
    }
}