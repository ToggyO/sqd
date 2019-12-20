using System.Collections.Generic;
using System.Linq;
using Mapper;
using Squadio.Domain.Models.Teams;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Users;

namespace Squadio.DTO.Teams
{
    public class TeamUserModelMapper : IMapper<TeamUserModel, TeamWithUserRoleDTO>
    {
        private readonly IMapper _mapper;

        public TeamUserModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public TeamWithUserRoleDTO Map(TeamUserModel item)
        {
            var result = new TeamWithUserRoleDTO
            {
                TeamId = item.TeamId,
                Status = (int) item.Status,
                StatusName = item.Status.ToString(),
                CreatedDate = item.CreatedDate
            };
            if (item.Team != null)
            {
                result.Team = _mapper.Map<TeamModel, TeamDTO>(item.Team);
            }
            return result;
        }
    }
    
    public class EnumerableTeamUserModelMapper : IMapper<IEnumerable<TeamUserModel>, IEnumerable<TeamWithUserRoleDTO>>
    {
        private readonly IMapper _mapper;

        public EnumerableTeamUserModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public IEnumerable<TeamWithUserRoleDTO> Map(IEnumerable<TeamUserModel> items)
        {
            var result = items.Select(x => _mapper.Map<TeamUserModel, TeamWithUserRoleDTO>(x));
            return result;
        }
    }
}