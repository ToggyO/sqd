using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Models.Invites;

namespace Squadio.BLL.Services.Membership
{
    public interface IMembershipService
    {
        Task<Response> InviteUsersToCompany(Guid companyId, Guid authorId, CreateInvitesDTO dto, bool sendMails = true);
        Task<Response> InviteUsersToTeam(Guid teamId, Guid authorId, CreateInvitesDTO dto, bool sendMails = true);
        Task<Response> InviteUsersToProject(Guid projectId, Guid authorId, CreateInvitesDTO dto, bool sendMails = true);
        
        Task<Response> DeleteUserFromCompany(Guid companyId, Guid removeUserId, Guid currentUserId);
        Task<Response> DeleteUserFromTeam(Guid teamId, Guid removeUserId, Guid currentUserId, bool checkAccess = true);
        Task<Response> DeleteUserFromProject(Guid projectId, Guid removeUserId, Guid currentUserId, bool checkAccess = true);
    }
}