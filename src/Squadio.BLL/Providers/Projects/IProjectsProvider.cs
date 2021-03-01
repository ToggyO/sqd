using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Models.Projects;
using Squadio.DTO.Models.Users;
using Squadio.DTO.Projects;

namespace Squadio.BLL.Providers.Projects
{
    public interface IProjectsProvider
    {
        Task<Response<PageModel<ProjectDTO>>> GetProjects(PageModel model, Guid? companyId = null, Guid? teamId = null);
        Task<Response<PageModel<ProjectWithUserRoleDTO>>> GetUserProjects(Guid userId, PageModel model, Guid? companyId = null, Guid? teamId = null, Guid? projectId = null);
        Task<Response<PageModel<UserWithRoleDTO>>> GetProjectUsers(Guid projectId, PageModel model);
        Task<Response<ProjectDTO>> GetById(Guid id);
    }
}