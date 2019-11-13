using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;

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
            company.Address = entity.Address;
            _context.Update(company);
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task<PageModel<CompanyModel>> GetCompanies(PageModel pageModel, CompaniesFilter filter = null, string search = null)
        {
            IQueryable<CompanyModel> query = _context.Companies;

            if (!string.IsNullOrEmpty(search))
            {
                var searchUpper = search.ToUpper();

                query = query.Where(x => x.Name.ToUpper().Contains(searchUpper));
            }

            if (filter != null)
            {
                if (filter.FromDate != null)
                {
                    query = query.Where(x => x.CreatedDate >= filter.FromDate);
                }
                
                if (filter.ToDate != null)
                {
                    query = query.Where(x => x.CreatedDate <= filter.ToDate);
                }
            }

            var skip = (pageModel.Page - 1) * pageModel.PageSize;
            var take = pageModel.PageSize;

            var total = await query.CountAsync();
            var items = await query
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return new PageModel<CompanyModel>
            {
                Page = pageModel.Page,
                PageSize = pageModel.PageSize,
                Total = total,
                Items = items
            };
        }
    }
}