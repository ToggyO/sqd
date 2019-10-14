using System;
using System.Threading.Tasks;
using Squadio.DTO.Projects;

namespace Squadio.BLL.Services.Projects
{
    public interface IProjectsService
    {
        Task<ProjectDTO> Create(Guid userId, CreateProjectDTO dto);
    }
}