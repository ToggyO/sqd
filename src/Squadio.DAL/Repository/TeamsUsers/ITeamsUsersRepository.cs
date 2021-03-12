using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squadio.Common.Models.Pages;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Teams;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.TeamsUsers
{
    public interface ITeamsUsersRepository
    {
        Task<PageModel<TeamUserModel>> GetTeamsUsers(PageModel model
            , Guid? userId = null
            , Guid? companyId = null
            , Guid? teamId = null
            , IEnumerable<MembershipStatus> statuses = null);
        Task<IEnumerable<TeamUserModel>> GetTeamUsersByEmails(Guid companyId, IEnumerable<string> emails);
        Task<TeamUserModel> GetTeamUser(Guid teamId, Guid userId);
        Task AddTeamUser(Guid teamId, Guid userId, MembershipStatus membershipStatus);
        Task DeleteTeamUser(Guid teamId, Guid userId);
    }
}