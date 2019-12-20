using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
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
            var item = await _context.Teams
                .Include(x => x.Company)
                .Include(x=>x.Creator)
                .FirstOrDefaultAsync(x => x.Id == id);
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
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<PageModel<TeamModel>> GetTeams(PageModel model, Guid? companyId = null)
        {
            IQueryable<TeamModel> query = _context.Teams;

            if (companyId.HasValue)
            {
                query = query.Where(x => x.CompanyId == companyId);
            }

            var items = await query
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToListAsync();

            var total = await query.CountAsync();

            var result = new PageModel<TeamModel>
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