using System;
using System.Collections.Generic;
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
        Task<Response> InviteUsers(Guid entityId, InviteEntityType entityType, Guid authorId, IEnumerable<string> emails, bool sendMails = true);
        
        Task<Response> DeleteUserFromCompany(Guid companyId, Guid removeUserId, Guid currentUserId);
        Task<Response> DeleteUserFromTeam(Guid teamId, Guid removeUserId, Guid currentUserId, bool checkAccess = true);
        Task<Response> DeleteUserFromProject(Guid projectId, Guid removeUserId, Guid currentUserId, bool checkAccess = true);
    }
}