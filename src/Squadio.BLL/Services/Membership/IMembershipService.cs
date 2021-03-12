using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Enums;
using Squadio.DTO.Models.Invites;
using Squadio.DTO.Models.Users;

namespace Squadio.BLL.Services.Membership
{
    public interface IMembershipService
    {
        Task<Response> ApplyInvite(Guid userId, Guid entityId, InviteEntityType entityType);
        Task<Response> AddUserToCompany(Guid companyId, Guid userId, MembershipStatus membershipStatus);
        Task<Response> AddUserToTeam(Guid teamId, Guid userId, MembershipStatus membershipStatus);
        Task<Response> AddUserToProject(Guid projectId, Guid userId, MembershipStatus membershipStatus);
        // Task<Response> InviteUsersToCompany(Guid companyId, Guid authorId, CreateInvitesDTO dto, bool sendMails = true);
        // Task<Response> InviteUsersToTeam(Guid teamId, Guid authorId, CreateInvitesDTO dto, bool sendMails = true);
        // Task<Response> InviteUsersToProject(Guid projectId, Guid authorId, CreateInvitesDTO dto, bool sendMails = true);
        // Task<Response<UserDTO>> InviteUserToCompany(Guid companyId, Guid inviteAuthorId, string email, bool sendMails = true);
        // Task<Response<UserDTO>> InviteUserToTeam(Guid teamId, Guid inviteAuthorId, string email, bool sendMails = true);
        // Task<Response<UserDTO>> InviteUserToProject(Guid projectId, Guid inviteAuthorId, string email, bool sendMails = true);
        
        Task<Response> DeleteUserFromCompany(Guid companyId, Guid removeUserId, Guid currentUserId);
        Task<Response> DeleteUserFromTeam(Guid teamId, Guid removeUserId, Guid currentUserId, bool checkAccess = true);
        Task<Response> DeleteUserFromProject(Guid projectId, Guid removeUserId, Guid currentUserId, bool checkAccess = true);
    }
}