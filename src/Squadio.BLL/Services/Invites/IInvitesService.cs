using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Invites;

namespace Squadio.BLL.Services.Invites
{
    public interface IInvitesService
    {
        Task<Response<IEnumerable<InviteDTO>>> InviteToTeam(Guid teamId, Guid authorId, CreateInvitesDTO dto);
        Task<Response<InviteDTO>> InviteToTeam(string authorName, string teamName, Guid teamId, string email);
        Task<Response<IEnumerable<InviteDTO>>> InviteToProject(Guid projectId, Guid authorId, CreateInvitesDTO dto);
        Task<Response<InviteDTO>> InviteToProject(string authorName, string projectName, Guid projectId, string email);
        Task<Response> AcceptInviteToTeam(Guid userId, Guid teamId, string code);
        Task<Response> AcceptInviteToProject(Guid userId, Guid projectId, string code);
    }
}