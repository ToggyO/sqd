using System;
using System.Threading.Tasks;
using Squadio.Domain.Models.Projects;

namespace Squadio.DAL.Repository.Projects.Implementation
{
    public class ProjectsRepository : IProjectsRepository
    {
        private readonly SquadioDbContext _context;
        public ProjectsRepository(SquadioDbContext context)
        {
            _context = context;
        }

        public async Task<ProjectModel> GetById(Guid id)
        {
            var item = await _context.Projects.FindAsync(id);
            return item;
        }

        public async Task<ProjectModel> Create(ProjectModel entity)
        {
            var item = _context.Projects.Add(entity);
            await _context.SaveChangesAsync();
            return item.Entity;
        }

        public Task<ProjectModel> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<ProjectModel> Update(ProjectModel entity)
        {
            var item = await _context.Projects.FindAsync(entity.Id);
            item.Name = entity.Name;
            _context.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }
    }
}