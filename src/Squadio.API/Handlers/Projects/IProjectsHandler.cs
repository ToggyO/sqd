using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Projects;

namespace Squadio.API.Handlers.Projects
{
    public interface IProjectsHandler
    {
        Task<Response<ProjectDTO>> GetById(Guid id);
        Task<Response<ProjectDTO>> Create(CreateProjectDTO dto, ClaimsPrincipal claims);
    }
}