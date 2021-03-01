using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Domain.Models.Resources;

namespace Squadio.DAL.Repository.Resources.Implementation
{
    public class ResourcesRepository : IResourcesRepository
    {
        private readonly SquadioDbContext _context;

        public ResourcesRepository(SquadioDbContext context)
        {
            _context = context;
        }

        public async Task<ResourceModel> Create(ResourceModel model)
        {
            var result = await _context.Resources.AddAsync(model);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<ResourceModel> GetById(Guid id)
        {
            var result = await _context.Resources
                .Include(x => x.User)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            return result;
        }

        public async Task<ResourceModel> Delete(Guid id)
        {
            var result = await _context.Resources.FindAsync(id);
            _context.Resources.Remove(result);
            await _context.SaveChangesAsync();
            return result;
        }

        public async Task<ResourceModel> GetByFilename(string filename)
        {
            var result = await _context.Resources.
                FirstOrDefaultAsync(x=> x.FileName.ToUpper() == filename.ToUpper());
            return result;
        }
    }
}