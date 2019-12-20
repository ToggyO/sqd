using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Projects;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.Projects
{
    public interface IProjectsProvider
    {
        Task<Response<PageModel<ProjectDTO>>> GetProjects(PageModel model, ProjectFilter filter);
        Task<Response<PageModel<ProjectWithUserRoleDTO>>> GetUserProjects(Guid userId, PageModel model, Guid? companyId = null, Guid? teamId = null);
        Task<Response<PageModel<UserWithRoleDTO>>> GetProjectUsers(Guid projectId, PageModel model);
        Task<Response<ProjectDTO>> GetById(Guid id);
    }
}