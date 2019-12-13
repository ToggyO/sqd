using System.Collections.Generic;
using System.Linq;
using Mapper;
using Squadio.Domain.Models.Projects;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Users;

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
            var result = new ProjectDTO
            {
                Id = item.Id,
                Name = item.Name,
                ColorHex = item.ColorHex,
                TeamId = item.TeamId
            };
            
            if (item.Creator != null)
            {
                result.Creator = _mapper.Map<UserModel, UserDTO>(item.Creator);
            }

            return result;
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