using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.BLL.Providers.Projects;
using Squadio.BLL.Providers.WebSocket.BaseHubProvider;
using Squadio.BLL.Services.Projects;
using Squadio.Common.Enums;
using Squadio.Common.Extensions;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.Common.Models.WebSocket;
using Squadio.Common.WebSocket;
using Squadio.DTO.Projects;

namespace Squadio.API.Handlers.Projects.Implementation
{
    public class ProjectsHandler : IProjectsHandler
    {
        private readonly IProjectsProvider _provider;
        private readonly IProjectsService _service;
        private readonly IBaseHubProvider _baseHubProvider;

        public ProjectsHandler(IProjectsProvider provider
            , IProjectsService service
            , IBaseHubProvider baseHubProvider)
        {
            _provider = provider;
            _service = service;
            _baseHubProvider = baseHubProvider;
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

        public async Task<Response<ProjectDTO>> Create(Guid teamId, ProjectCreateDTO dto, ClaimsPrincipal claims)
        {
            var result = await _service.Create(claims.GetUserId(), teamId, dto);
            await BroadcastChanges(result);
            return result;
        }

        public async Task<Response<ProjectDTO>> Update(Guid projectId, ProjectUpdateDTO dto, ClaimsPrincipal claims)
        {
            var result = await _service.Update(projectId, claims.GetUserId(), dto);
            await BroadcastChanges(result);
            return result;
        }

        public async Task<Response> Delete(Guid projectId, ClaimsPrincipal claims)
        {
            var result = await _service.Delete(projectId, claims.GetUserId());
            await BroadcastChanges(result);
            return result;
        }

        public async Task<Response> DeleteProjectUser(Guid projectId, Guid userId, ClaimsPrincipal claims)
        {
            var result = await _service.DeleteUserFromProject(projectId, userId, claims.GetUserId());
            return result;
        }

        private async Task BroadcastChanges(Response<ProjectDTO> response)
        {
            if (response.IsSuccess)
            {
                await _baseHubProvider.BroadcastSidebarChanges(
                    response.Data.TeamId, 
                    ConnectionGroup.Sidebar, 
                    EndpointsWS.Sidebar.Broadcast,
                    new BroadcastSidebarProjectChangesModel
                    {
                        TeamId = response.Data.TeamId,
                        ProjectId = response.Data.Id
                    });
            }
        }
    }
}