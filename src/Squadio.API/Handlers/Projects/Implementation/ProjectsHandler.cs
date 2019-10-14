using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.BLL.Providers.Projects;
using Squadio.BLL.Services.Projects;
using Squadio.Common.Exceptions.PermissionException;
using Squadio.Common.Extensions;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Projects;

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

        public async Task<Response<ProjectDTO>> GetById(Guid id)
        {
            var item = await _provider.GetById(id);
            var result = new Response<ProjectDTO>
            {
                Data = item
            };
            return result;
        }

        public async Task<Response<ProjectDTO>> Create(CreateProjectDTO dto, ClaimsPrincipal claims)
        {
            var item = await _service.Create(claims.GetUserId() ?? throw new PermissionException(), dto);
            var result = new Response<ProjectDTO>
            {
                Data = item
            };
            return result;
        }
    }
}