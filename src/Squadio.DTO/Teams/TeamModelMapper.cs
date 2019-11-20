using System.Collections.Generic;
using System.Linq;
using Mapper;
using Squadio.Domain.Models.Teams;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Users;

namespace Squadio.DTO.Teams
{
    public class TeamModelMapper: IMapper<TeamModel, TeamDTO>
    {
        private readonly IMapper _mapper;

        public TeamModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public TeamDTO Map(TeamModel item)
        {
            var result = new TeamDTO
            {
                Id = item.Id,
                Name = item.Name
            };
            
            if (item.Creator != null)
            {
                result.Creator = _mapper.Map<UserModel, UserDTO>(item.Creator);
            }

            return result;
        }
    }
    
    public class EnumerableTeamModelMapper : IMapper<IEnumerable<TeamModel>, IEnumerable<TeamDTO>>
    {
        private readonly IMapper _mapper;

        public EnumerableTeamModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public IEnumerable<TeamDTO> Map(IEnumerable<TeamModel> items)
        {
            var result = items.Select(x => _mapper.Map<TeamModel, TeamDTO>(x));
            return result;
        }
    }
}