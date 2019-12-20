using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Domain.Models.Projects;

namespace Squadio.DAL.Repository.Projects
{
    public interface IProjectsRepository : IBaseRepository<ProjectModel>
    {
        Task<PageModel<ProjectModel>> GetProjects(PageModel model, Guid? companyId = null, Guid? teamId = null);
    }
}