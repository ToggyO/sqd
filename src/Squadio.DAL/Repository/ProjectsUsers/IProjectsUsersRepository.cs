using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Projects;

namespace Squadio.DAL.Repository.ProjectsUsers
{
    public interface IProjectsUsersRepository
    {
        Task<ProjectUserModel> GetProjectUser(Guid projectId, Guid userId);
        Task AddProjectUser(Guid projectId, Guid userId, UserStatus userStatus);
        Task AddRangeProjectUser(Guid projectId, IEnumerable<Guid> userIds, UserStatus userStatus);
        Task ChangeStatusProjectUser(Guid projectId, Guid userId, UserStatus newUserStatus);
    }
}