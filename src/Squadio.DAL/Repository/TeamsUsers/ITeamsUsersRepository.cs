using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squadio.Domain.Enums;

namespace Squadio.DAL.Repository.TeamsUsers
{
    public interface ITeamsUsersRepository
    {
        Task AddTeamUser(Guid teamId, Guid userId, UserStatus userStatus);
        Task AddRangeTeamUser(Guid teamId, IEnumerable<Guid> userIds, UserStatus userStatus);
        Task ChangeStatusTeamUser(Guid teamId, Guid userId, UserStatus newUserStatus);
    }
}