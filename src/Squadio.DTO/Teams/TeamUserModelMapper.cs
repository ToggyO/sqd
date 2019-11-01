using System.Collections.Generic;
using System.Linq;
using Mapper;
using Squadio.Domain.Models.Teams;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Users;

namespace Squadio.DTO.Teams
{
    public class TeamUserModelMapper : IMapper<TeamUserModel, TeamUserDTO>
    {
        private readonly IMapper _mapper;

        public TeamUserModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public TeamUserDTO Map(TeamUserModel item)
        {
            var result = new TeamUserDTO
            {
                UserId = item.UserId,
                TeamId = item.TeamId,
                Status = (int) item.Status,
                StatusName = item.Status.ToString(),
                CreatedDate = item.CreatedDate
            };
            if (item.User != null)
            {
                result.User = _mapper.Map<UserModel, UserDTO>(item.User);
            }
            if (item.Team != null)
            {
                result.Team = _mapper.Map<TeamModel, TeamDTO>(item.Team);
            }
            return result;
        }
    }
    
    public class EnumerableTeamUserModelMapper : IMapper<IEnumerable<TeamUserModel>, IEnumerable<TeamUserDTO>>
    {
        private readonly IMapper _mapper;

        public EnumerableTeamUserModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public IEnumerable<TeamUserDTO> Map(IEnumerable<TeamUserModel> items)
        {
            var result = items.Select(x => _mapper.Map<TeamUserModel, TeamUserDTO>(x));
            return result;
        }
    }
}