using Squadio.DAL.Repository.Teams;

namespace Squadio.BLL.Providers.Teams.Implementation
{
    public class TeamsProvider : ITeamsProvider
    {
        private readonly ITeamsRepository _repository;

        public TeamsProvider(ITeamsRepository repository)
        {
            _repository = repository;
        }
    }
}