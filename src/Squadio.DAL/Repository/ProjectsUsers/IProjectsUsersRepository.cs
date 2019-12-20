using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Projects;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.ProjectsUsers
{
    public interface IProjectsUsersRepository
    {
        Task<PageModel<ProjectUserModel>> GetProjectsUsers(PageModel model
            , Guid? userId = null
            , Guid? companyId = null
            , Guid? teamId = null
            , Guid? projectId = null
            , IEnumerable<UserStatus> statuses = null);
        Task<ProjectUserModel> GetProjectUser(Guid projectId, Guid userId);
        Task<ProjectUserModel> GetFullProjectUser(Guid projectId, Guid userId);
        Task AddProjectUser(Guid projectId, Guid userId, UserStatus userStatus);
        Task DeleteProjectUser(Guid projectId, Guid userId);
        Task DeleteProjectUsers(Guid projectId, IEnumerable<string> emails);
        Task AddRangeProjectUser(Guid projectId, IEnumerable<Guid> userIds, UserStatus userStatus);
        Task ChangeStatusProjectUser(Guid projectId, Guid userId, UserStatus newUserStatus);
    }
}