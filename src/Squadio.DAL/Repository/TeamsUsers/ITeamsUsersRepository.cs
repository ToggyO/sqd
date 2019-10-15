using System;
using System.Threading.Tasks;
using Squadio.Domain.Enums;

namespace Squadio.DAL.Repository.TeamsUsers
{
    public interface ITeamsUsersRepository
    {
        Task AddTeamUser(Guid teamId, Guid userId, UserStatus userStatus);
        Task ChangeStatusTeamUser(Guid teamId, Guid userId, UserStatus newUserStatus);
    }
}