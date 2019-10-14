using System;
using System.Threading.Tasks;
using Squadio.Domain.Models.Teams;

namespace Squadio.DAL.Repository.Teams.Implementation
{
    public class TeamsRepository : ITeamsRepository
    {
        private readonly SquadioDbContext _context;
        public TeamsRepository(SquadioDbContext context)
        {
            _context = context;
        }

        public Task<TeamModel> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<TeamModel> Create(TeamModel entity)
        {
            throw new NotImplementedException();
        }

        public Task<TeamModel> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<TeamModel> Update(TeamModel entity)
        {
            throw new NotImplementedException();
        }
    }
}