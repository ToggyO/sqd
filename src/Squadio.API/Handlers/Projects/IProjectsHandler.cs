using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Projects;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.Projects
{
    public interface IProjectsHandler
    {
        Task<Response<PageModel<ProjectDTO>>> GetProjects(PageModel model, ProjectFilter filter);
        Task<Response<PageModel<ProjectUserDTO>>> GetProjectUsers(Guid projectId, PageModel model);
        Task<Response<ProjectDTO>> GetById(Guid id);
        Task<Response<ProjectDTO>> Create(Guid teamId, CreateProjectDTO dto, ClaimsPrincipal claims);
        Task<Response<ProjectDTO>> Update(Guid projectId, ProjectUpdateDTO dto, ClaimsPrincipal claims);
        Task<Response> Delete(Guid projectId, ClaimsPrincipal claims);
        Task<Response> DeleteProjectUser(Guid projectId, Guid userId, ClaimsPrincipal claims);
    }
}