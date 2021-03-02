using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.BLL.Providers.Projects;
using Squadio.BLL.Services.Membership;
using Squadio.BLL.Services.Projects;
using Squadio.Common.Extensions;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Models.Invites;
using Squadio.DTO.Models.Projects;
using Squadio.DTO.Models.Users;
using Squadio.DTO.Projects;

namespace Squadio.API.Handlers.Projects.Implementations
{
    public class ProjectsHandler : IProjectsHandler
    {
        private readonly IProjectsProvider _provider;
        private readonly IProjectsService _service;
        private readonly IMembershipService _membershipService;
        // private readonly ISidebarHubProvider _hubProvider;

        public ProjectsHandler(IProjectsProvider provider
            , IProjectsService service
            , IMembershipService membershipService
            // , ISidebarHubProvider hubProvider
            )
        {
            _provider = provider;
            _service = service;
            _membershipService = membershipService;
            // _hubProvider = hubProvider;
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
            // await BroadcastChanges(result, ChangesType.Created);
            return result;
        }

        public async Task<Response<ProjectDTO>> Update(Guid projectId, ProjectUpdateDTO dto, ClaimsPrincipal claims)
        {
            var result = await _service.Update(projectId, claims.GetUserId(), dto);
            // await BroadcastChanges(result, ChangesType.Updated);
            return result;
        }

        public async Task<Response> Delete(Guid projectId, ClaimsPrincipal claims)
        {
            var result = await _service.Delete(projectId, claims.GetUserId());
            // await BroadcastChanges(result, ChangesType.Deleted);
            return result;
        }

        public async Task<Response> DeleteProjectUser(Guid projectId, Guid userId, ClaimsPrincipal claims)
        {
            var result = await _membershipService.DeleteUserFromProject(projectId, userId, claims.GetUserId());
            return result;
        }

        public async Task<Response> InviteProjectUsers(Guid projectId, CreateInvitesDTO dto, ClaimsPrincipal claims)
        {
            var result = await _membershipService.InviteUsersToProject(projectId, claims.GetUserId(), dto);
            return result;
        }

        // private async Task BroadcastChanges(Response<ProjectDTO> response, ChangesType changesType)
        // {
        //     if (response.IsSuccess)
        //     {
        //         await _hubProvider.BroadcastProjectChanges(response.Data.TeamId, 
        //             new BroadcastSidebarProjectChangesModel
        //             {
        //                 TeamId = response.Data.TeamId,
        //                 ProjectId = response.Data.Id,
        //                 ChangesType = changesType
        //             });
        //     }
        // }
    }
}