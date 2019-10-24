using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Pages;
using Squadio.DTO.Projects;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.Projects
{
    public interface IProjectsHandler
    {
        Task<Response<PageModel<UserDTO>>> GetProjectUsers(Guid projectId, PageModel model);
        Task<Response<ProjectDTO>> GetById(Guid id);
        Task<Response<ProjectDTO>> Create(Guid companyId, CreateProjectDTO dto, ClaimsPrincipal claims);
    }
}