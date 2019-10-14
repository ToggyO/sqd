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

        public async Task<TeamModel> GetById(Guid id)
        {
            var item = await _context.Teams.FindAsync(id);
            return item;
        }

        public async Task<TeamModel> Create(TeamModel entity)
        {
            var item = _context.Teams.Add(entity);
            await _context.SaveChangesAsync();
            return item.Entity;
        }

        public Task<TeamModel> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<TeamModel> Update(TeamModel entity)
        {
            var item = await _context.Teams.FindAsync(entity.Id);
            item.Name = entity.Name;
            _context.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }
    }
}