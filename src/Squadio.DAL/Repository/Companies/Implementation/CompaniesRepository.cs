using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;
using Squadio.DTO.Pages;

namespace Squadio.DAL.Repository.Companies.Implementation
{
    public class CompaniesRepository : ICompaniesRepository
    {
        private readonly SquadioDbContext _context;
        public CompaniesRepository(SquadioDbContext context)
        {
            _context = context;
        }
        public async Task<CompanyModel> GetById(Guid id)
        {
            var entity = await _context.Companies.FindAsync(id);
            return entity;
        }

        public async Task<CompanyModel> Create(CompanyModel entity)
        {
            var company = await _context.Companies.AddAsync(entity);
            await _context.SaveChangesAsync();
            var result = company.Entity;
            return result;
        }

        public async Task<CompanyModel> Delete(Guid id)
        {
            var entity = await _context.Companies.FindAsync(id);
            _context.Companies.Remove(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<CompanyModel> Update(CompanyModel entity)
        {
            var company = await _context.Companies.FindAsync(entity.Id);
            company.Name = entity.Name;
            _context.Update(company);
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task<PageModel<CompanyModel>> GetCompaniesOfUser(Guid userId, PageModel model, UserStatus? status = null)
        {
            var query = _context.CompaniesUsers
                .Where(x => x.UserId == userId);

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status);
            }
            
            var total = await query.CountAsync();
            var items = await query
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize)
                .Include(x => x.Company)
                .Select(x => x.Company)
                .ToListAsync();
            
            var result = new PageModel<CompanyModel>
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