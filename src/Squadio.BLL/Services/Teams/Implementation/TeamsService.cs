using Squadio.DAL.Repository.Teams;

namespace Squadio.BLL.Services.Teams.Implementation
{
    public class TeamsService : ITeamsService
    {
        private readonly ITeamsRepository _repository;

        public TeamsService(ITeamsRepository repository)
        {
            _repository = repository;
        }
    }
}