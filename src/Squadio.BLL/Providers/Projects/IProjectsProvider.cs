using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Pages;
using Squadio.DTO.Projects;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.Projects
{
    public interface IProjectsProvider
    {
        Task<Response<PageModel<UserDTO>>> GetProjectUsers(Guid projectId, PageModel model);
        Task<Response<ProjectDTO>> GetById(Guid id);
    }
}