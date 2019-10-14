using Mapper;
using Squadio.DAL.Repository.Projects;

namespace Squadio.BLL.Services.Projects.Implementation
{
    public class ProjectsService : IProjectsService
    {
        private readonly IProjectsRepository _repository;
        private readonly IMapper _mapper;

        public ProjectsService(IProjectsRepository repository
            , IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
    }
}