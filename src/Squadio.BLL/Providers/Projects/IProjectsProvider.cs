using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Projects;

namespace Squadio.BLL.Providers.Projects
{
    public interface IProjectsProvider
    {
        Task<Response<ProjectDTO>> GetById(Guid id);
    }
}