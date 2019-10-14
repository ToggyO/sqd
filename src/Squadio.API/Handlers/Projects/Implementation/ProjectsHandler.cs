using Squadio.BLL.Providers.Projects;
using Squadio.BLL.Services.Projects;

namespace Squadio.API.Handlers.Projects.Implementation
{
    public class ProjectsHandler : IProjectsHandler
    {
        private readonly IProjectsProvider _provider;
        private readonly IProjectsService _service;

        public ProjectsHandler(IProjectsProvider provider
            , IProjectsService service)
        {
            _provider = provider;
            _service = service;
        }
    }
}