using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Projects;

namespace Squadio.BLL.Services.Projects
{
    public interface IProjectsService
    {
        Task<Response<ProjectDTO>> Create(Guid userId, CreateProjectDTO dto);
    }
}