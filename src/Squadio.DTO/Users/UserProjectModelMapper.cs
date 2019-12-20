using System.Collections.Generic;
using System.Linq;
using Mapper;
using Squadio.Domain.Models.Projects;
using Squadio.Domain.Models.Users;

namespace Squadio.DTO.Users
{
    public class UserProjectModelMapper : IMapper<ProjectUserModel, UserWithRoleDTO>
    {
        private readonly IMapper _mapper;

        public UserProjectModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public UserWithRoleDTO Map(ProjectUserModel item)
        {
            var result = new UserWithRoleDTO
            {
                UserId = item.UserId,
                Status = (int) item.Status,
                StatusName = item.Status.ToString(),
                CreatedDate = item.CreatedDate
            };
            if (item.User != null)
            {
                result.User = _mapper.Map<UserModel, UserDTO>(item.User);
            }
            return result;
        }
    }
    
    public class EnumerableUserProjectModelMapper : IMapper<IEnumerable<ProjectUserModel>, IEnumerable<UserWithRoleDTO>>
    {
        private readonly IMapper _mapper;

        public EnumerableUserProjectModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public IEnumerable<UserWithRoleDTO> Map(IEnumerable<ProjectUserModel> items)
        {
            var result = items.Select(x => _mapper.Map<ProjectUserModel, UserWithRoleDTO>(x));
            return result;
        }
    }
}