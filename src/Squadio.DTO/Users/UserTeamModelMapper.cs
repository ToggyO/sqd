using System.Collections.Generic;
using System.Linq;
using Mapper;
using Squadio.Domain.Models.Teams;
using Squadio.Domain.Models.Users;

namespace Squadio.DTO.Users
{
    public class UserTeamModelMapper : IMapper<TeamUserModel, UserWithRoleDTO>
    {
        private readonly IMapper _mapper;

        public UserTeamModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public UserWithRoleDTO Map(TeamUserModel item)
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
    
    public class EnumerableUserTeamModelMapper : IMapper<IEnumerable<TeamUserModel>, IEnumerable<UserWithRoleDTO>>
    {
        private readonly IMapper _mapper;

        public EnumerableUserTeamModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public IEnumerable<UserWithRoleDTO> Map(IEnumerable<TeamUserModel> items)
        {
            var result = items.Select(x => _mapper.Map<TeamUserModel, UserWithRoleDTO>(x));
            return result;
        }
    }
}