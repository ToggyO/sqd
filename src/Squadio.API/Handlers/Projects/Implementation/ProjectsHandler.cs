using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.BLL.Providers.Projects;
using Squadio.BLL.Providers.WebSocket.Sidebar;
using Squadio.BLL.Services.Projects;
using Squadio.Common.Enums;
using Squadio.Common.Extensions;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.Common.WebSocket;
using Squadio.DTO.Invites;
using Squadio.DTO.Projects;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.Projects.Implementation
{
    public class ProjectsHandler : IProjectsHandler
    {
        private readonly IProjectsProvider _provider;
        private readonly IProjectsService _service;
        private readonly ISidebarHubProvider _hubProvider;
        private readonly IProjectInvitesService _projectInvitesService;

        public ProjectsHandler(IProjectsProvider provider
            , IProjectsService service
            , ISidebarHubProvider hubProvider
            , IProjectInvitesService projectInvitesService)
        {
            _provider = provider;
            _service = service;
            _hubProvider = hubProvider;
            _projectInvitesService = projectInvitesService;
        }

        public async Task<Response<PageModel<ProjectDTO>>> GetProjects(PageModel model, Guid? companyId = null, Guid? teamId = null)
        {
            var result = await _provider.GetProjects(model, companyId, teamId);
            return result;
        }

        public async Task<Response<PageModel<UserWithRoleDTO>>> GetProjectUsers(Guid projectId, PageModel model)
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
            await BroadcastChanges(result, ChangesType.Created);
            return result;
        }

        public async Task<Response<ProjectDTO>> Update(Guid projectId, ProjectUpdateDTO dto, ClaimsPrincipal claims)
        {
            var result = await _service.Update(projectId, claims.GetUserId(), dto);
            await BroadcastChanges(result, ChangesType.Updated);
            return result;
        }

        public async Task<Response> Delete(Guid projectId, ClaimsPrincipal claims)
        {
            var result = await _service.Delete(projectId, claims.GetUserId());
            await BroadcastChanges(result, ChangesType.Deleted);
            return result;
        }

        public async Task<Response> DeleteProjectUser(Guid projectId, Guid userId, ClaimsPrincipal claims)
        {
            var result = await _service.DeleteUserFromProject(projectId, userId, claims.GetUserId());
            return result;
        }

        public async Task<Response> CreateInvite(Guid projectId, CreateInvitesDTO dto, ClaimsPrincipal claims)
        {
            var result = await _projectInvitesService.CreateInvite(projectId, claims.GetUserId(), dto);
            return result;
        }

        public async Task<Response> CancelInvite(Guid projectId, CancelInvitesDTO dto, ClaimsPrincipal claims)
        {
            var result = await _projectInvitesService.CancelInvite(projectId, claims.GetUserId(), dto);
            return result;
        }

        public async Task<Response> AcceptInvite(ClaimsPrincipal claims, string code)
        {
            var result = await _projectInvitesService.AcceptInvite(claims.GetUserId(), code);
            return result;
        }

        private async Task BroadcastChanges(Response<ProjectDTO> response, ChangesType changesType)
        {
            if (response.IsSuccess)
            {
                await _hubProvider.BroadcastProjectChanges(response.Data.TeamId, 
                    new BroadcastSidebarProjectChangesModel
                    {
                        TeamId = response.Data.TeamId,
                        ProjectId = response.Data.Id,
                        ChangesType = changesType
                    });
            }
        }
    }
}