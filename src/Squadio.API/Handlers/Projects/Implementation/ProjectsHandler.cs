using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.BLL.Providers.Projects;
using Squadio.BLL.Services.Projects;
using Squadio.Common.Extensions;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Projects;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.Projects.Implementation
{
    public class ProjectsHandler : IProjectsHandler
    {
        private readonly IProjectsProvider _provider;
        private readonly IProjectsService _service;

        public ProjectsHandler(IProjectsProvider provider
            , IProjectsService service)
        {
            _provider = provider;
            _service = service;
        }

        public async Task<Response<PageModel<ProjectDTO>>> GetProjects(PageModel model, ProjectFilter filter)
        {
            var result = await _provider.GetProjects(model, filter);
            return result;
        }

        public async Task<Response<PageModel<ProjectUserDTO>>> GetProjectUsers(Guid projectId, PageModel model)
        {
            var result = await _provider.GetProjectUsers(projectId, model);
            return result;
        }

        public async Task<Response<ProjectDTO>> GetById(Guid id)
        {
            var result = await _provider.GetById(id);
            return result;
        }

        public async Task<Response<ProjectDTO>> Create(Guid teamId, CreateProjectDTO dto, ClaimsPrincipal claims)
        {
            var result = await _service.Create(claims.GetUserId(), teamId, dto);
            return result;
        }

        public async Task<Response<ProjectDTO>> Update(Guid projectId, ProjectUpdateDTO dto, ClaimsPrincipal claims)
        {
            var result = await _service.Update(projectId, claims.GetUserId(), dto);
            return result;
        }

        public async Task<Response> Delete(Guid projectId, ClaimsPrincipal claims)
        {
            var result = await _service.Delete(projectId, claims.GetUserId());
            return result;
        }

        public async Task<Response> DeleteProjectUser(Guid projectId, Guid userId, ClaimsPrincipal claims)
        {
            var result = await _service.DeleteUserFromProject(projectId, userId, claims.GetUserId());
            return result;
        }
    }
}