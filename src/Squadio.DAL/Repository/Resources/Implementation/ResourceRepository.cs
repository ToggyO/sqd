using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Domain.Models.Resources;

namespace Squadio.DAL.Repository.Resources.Implementation
{
    public class ResourceRepository : IResourceRepository
    {
        private readonly SquadioDbContext _context;

        public ResourceRepository(SquadioDbContext context)
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

        public async Task<ResourceModel> GetByFilename(string filename)
        {
            var result = await _context.Resources.
                FirstOrDefaultAsync(x=> x.FileName.ToUpper() == filename.ToUpper());
            return result;
        }
    }
}