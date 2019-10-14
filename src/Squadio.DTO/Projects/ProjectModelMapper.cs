using System.Collections.Generic;
using System.Linq;
using Mapper;
using Squadio.Domain.Models.Projects;

namespace Squadio.DTO.Projects
{
    public class ProjectModelMapper: IMapper<ProjectModel, ProjectDTO>
    {
        private readonly IMapper _mapper;

        public ProjectModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public ProjectDTO Map(ProjectModel item)
        {
            return new ProjectDTO
            {
                Id = item.Id,
                Name = item.Name
            };
        }
    }
    
    public class EnumerableProjectModelMapper : IMapper<IEnumerable<ProjectModel>, IEnumerable<ProjectDTO>>
    {
        private readonly IMapper _mapper;

        public EnumerableProjectModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public IEnumerable<ProjectDTO> Map(IEnumerable<ProjectModel> items)
        {
            var result = items.Select(x => _mapper.Map<ProjectModel, ProjectDTO>(x));
            return result;
        }
    }
}