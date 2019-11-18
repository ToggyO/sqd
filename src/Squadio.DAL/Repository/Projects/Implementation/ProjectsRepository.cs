using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Common.Models.Pages;
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
            var item = await _context.Projects
                .Include(x => x.Team)
                .ThenInclude(x => x.Company)
                .FirstOrDefaultAsync(x => x.Id == id);
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
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<PageModel<ProjectModel>> GetProjects(PageModel model, Guid? companyId = null, Guid? teamId = null)
        {
            IQueryable<ProjectModel> query = _context.Projects;

            if (companyId.HasValue)
            {
                query = query.Where(x => x.Team.CompanyId == companyId);
            }

            if (teamId.HasValue)
            {
                query = query.Where(x => x.TeamId == teamId);
            }
            
            var items = await query
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToListAsync();
            
            var total = await query.CountAsync();
            
            var result = new PageModel<ProjectModel>
            {
                Page = model.Page,
                PageSize = model.PageSize,
                Total = total,
                Items = items
            };
            return result;
        }
    }
}