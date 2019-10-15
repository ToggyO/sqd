using System;
using System.Threading.Tasks;
using Squadio.Domain.Enums;

namespace Squadio.DAL.Repository.ProjectsUsers
{
    public interface IProjectsUsersRepository
    {
        Task AddProjectUser(Guid projectId, Guid userId, UserStatus userStatus);
        Task ChangeStatusProjectUser(Guid projectId, Guid userId, UserStatus newUserStatus);
    }
}