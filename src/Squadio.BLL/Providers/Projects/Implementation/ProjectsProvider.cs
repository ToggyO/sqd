using System;
using System.Threading.Tasks;
using Mapper;
using Squadio.DAL.Repository.Projects;
using Squadio.Domain.Models.Projects;
using Squadio.DTO.Projects;

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

        public async Task<ProjectDTO> GetById(Guid id)
        {
            
            var entity = await _repository.GetById(id);

            var result = _mapper.Map<ProjectModel, ProjectDTO>(entity);

            return result;
        }
    }
}