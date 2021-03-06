using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Models.Invites;
using Squadio.DTO.Models.Projects;
using Squadio.DTO.Models.Users;
using Squadio.DTO.Projects;

namespace Squadio.API.Handlers.Projects
{
    public interface IProjectsHandler
    {
        Task<Response<PageModel<ProjectDTO>>> GetProjects(PageModel model, Guid? companyId = null, Guid? teamId = null);
        Task<Response<PageModel<UserWithRoleDTO>>> GetProjectUsers(Guid projectId, PageModel model);
        Task<Response<ProjectDTO>> GetById(Guid id);
        Task<Response<ProjectDTO>> Create(Guid teamId, ProjectCreateDTO dto, ClaimsPrincipal claims);
        Task<Response<ProjectDTO>> Update(Guid projectId, ProjectUpdateDTO dto, ClaimsPrincipal claims);
        Task<Response> Delete(Guid projectId, ClaimsPrincipal claims);
        Task<Response> DeleteProjectUser(Guid projectId, Guid userId, ClaimsPrincipal claims);
        Task<Response> InviteProjectUsers(Guid projectId, CreateInvitesDTO dto, ClaimsPrincipal claims);
    }
}