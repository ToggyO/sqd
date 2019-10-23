using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.BLL.Providers.Projects;
using Squadio.BLL.Services.Projects;
using Squadio.Common.Extensions;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Pages;
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

        public async Task<Response<PageModel<UserDTO>>> GetProjectUsers(Guid projectId, PageModel model)
        {
            var result = await _provider.GetProjectUsers(projectId, model);
            return result;
        }

        public async Task<Response<ProjectDTO>> GetById(Guid id)
        {
            var result = await _provider.GetById(id);
            return result;
        }

        public async Task<Response<ProjectDTO>> Create(CreateProjectDTO dto, ClaimsPrincipal claims)
        {
            var userId = claims.GetUserId();
            if (!userId.HasValue)
            {
                return claims.Unauthorized<ProjectDTO>();
            }
            var result = await _service.Create(userId.Value, dto);
            return result;
        }
    }
}