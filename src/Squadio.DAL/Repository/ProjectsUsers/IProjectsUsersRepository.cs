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
        Task<PageModel<ProjectUserModel>> GetUserProjects(Guid userId, PageModel model, Guid? companyId = null);
        Task<PageModel<ProjectUserModel>> GetProjectUsers(Guid projectId, PageModel model);
        Task<ProjectUserModel> GetProjectUser(Guid projectId, Guid userId);
        Task AddProjectUser(Guid projectId, Guid userId, UserStatus userStatus);
        Task AddRangeProjectUser(Guid projectId, IEnumerable<Guid> userIds, UserStatus userStatus);
        Task ChangeStatusProjectUser(Guid projectId, Guid userId, UserStatus newUserStatus);
    }
}