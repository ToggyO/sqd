using Mapper;
using Squadio.DAL.Repository.Teams;

namespace Squadio.BLL.Services.Teams.Implementation
{
    public class TeamsService : ITeamsService
    {
        private readonly ITeamsRepository _repository;
        private readonly IMapper _mapper;

        public TeamsService(ITeamsRepository repository
            , IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
    }
}