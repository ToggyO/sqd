using Mapper;
using Squadio.DAL.Repository.Projects;

namespace Squadio.BLL.Providers.Projects.Implementation
{
    public class ProjectsProvider : IProjectsProvider
    {
        private readonly IProjectsRepository _repository;
        private readonly IMapper _mapper;

        public ProjectsProvider(IProjectsRepository repository
            , IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
    }
}