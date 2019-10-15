using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Invites;

namespace Squadio.BLL.Services.Invites
{
    public interface IInvitesService
    {
        Task<Response> InviteToTeam(string authorName, string teamName, Guid teamId, string email);
        Task<Response> InviteToProject(string authorName, string projectName, Guid projectId, string email);
        Task<Response> AcceptInviteToTeam(Guid userId, Guid teamId, string code);
        Task<Response> AcceptInviteToProject(Guid userId, Guid projectId, string code);
    }
}