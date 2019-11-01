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
        Task<PageModel<TeamUserModel>> GetUserTeams(Guid userId, PageModel model, Guid? companyId = null);
        Task<PageModel<TeamUserModel>> GetTeamUsers(Guid teamId, PageModel model);
        Task<TeamUserModel> GetTeamUser(Guid teamId, Guid userId);
        Task AddTeamUser(Guid teamId, Guid userId, UserStatus userStatus);
        Task AddRangeTeamUser(Guid teamId, IEnumerable<Guid> userIds, UserStatus userStatus);
        Task ChangeStatusTeamUser(Guid teamId, Guid userId, UserStatus newUserStatus);
    }
}