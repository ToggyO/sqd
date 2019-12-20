using System.Collections.Generic;
using System.Linq;
using Mapper;
using Squadio.Domain.Models.Projects;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Users;

namespace Squadio.DTO.Projects
{
    public class ProjectUserModelMapper : IMapper<ProjectUserModel, ProjectWithUserRoleDTO>
    {
        private readonly IMapper _mapper;

        public ProjectUserModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public ProjectWithUserRoleDTO Map(ProjectUserModel item)
        {
            var result = new ProjectWithUserRoleDTO
            {
                ProjectId = item.ProjectId,
                Status = (int) item.Status,
                StatusName = item.Status.ToString(),
                CreatedDate = item.CreatedDate
            };
            if (item.Project != null)
            {
                result.Project = _mapper.Map<ProjectModel, ProjectDTO>(item.Project);
            }
            return result;
        }
    }
    
    public class EnumerableProjectUserModelMapper : IMapper<IEnumerable<ProjectUserModel>, IEnumerable<ProjectWithUserRoleDTO>>
    {
        private readonly IMapper _mapper;

        public EnumerableProjectUserModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public IEnumerable<ProjectWithUserRoleDTO> Map(IEnumerable<ProjectUserModel> items)
        {
            var result = items.Select(x => _mapper.Map<ProjectUserModel, ProjectWithUserRoleDTO>(x));
            return result;
        }
    }
}