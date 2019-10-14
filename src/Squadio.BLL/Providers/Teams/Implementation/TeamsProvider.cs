using System;
using System.Threading.Tasks;
using Mapper;
using Squadio.DAL.Repository.Teams;
using Squadio.Domain.Models.Teams;
using Squadio.DTO.Teams;

namespace Squadio.BLL.Providers.Teams.Implementation
{
    public class TeamsProvider : ITeamsProvider
    {
        private readonly ITeamsRepository _repository;
        private readonly IMapper _mapper;

        public TeamsProvider(ITeamsRepository repository
            , IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<TeamDTO> GetById(Guid id)
        {
            var entity = await _repository.GetById(id);

            var result = _mapper.Map<TeamModel, TeamDTO>(entity);

            return result;
        }
    }
}