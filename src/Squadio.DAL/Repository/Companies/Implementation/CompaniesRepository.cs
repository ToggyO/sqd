using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Common.Enums.Sorts;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Sorts;
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
            var entity = await _context.Companies
                .Include(x => x.Creator).ThenInclude(x=>x.Avatar)
                .FirstOrDefaultAsync(x => x.Id == id);
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
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<PageModel<CompanyModel>> GetCompanies(PageModel pageModel, CompanyFilterModel filter = null)
        {
            var query = _context.Companies
                .Include(x => x.Creator)
                .AsQueryable();

            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.Search))
                {
                    var searchUpper = filter.Search.ToUpper();
                    query = query.Where(x => x.Name.ToUpper().Contains(searchUpper));
                }
                if (filter.CreateFrom.HasValue)
                {
                    query = query.Where(x => x.CreatedDate >= filter.CreateFrom);
                }
                if (filter.CreateTo.HasValue)
                {
                    query = query.Where(x => x.CreatedDate <= filter.CreateTo);
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

        // public async Task<PageModel<CompanyModel>> GetCompanies(PageModel pageModel
        //     , CompanyAdminFilter filter = null
        //     , SortCompaniesModel sort = null
        //     , string search = null)
        // {
        //     IQueryable<CompanyModel> query = _context.Companies;
        //
        //     if (!string.IsNullOrEmpty(search))
        //     {
        //         var searchUpper = search.ToUpper();
        //
        //         query = query.Where(x => x.Name.ToUpper().Contains(searchUpper));
        //     }
        //
        //     if (filter != null)
        //     {
        //         if (filter.FromDate != null)
        //         {
        //             query = query.Where(x => x.CreatedDate >= filter.FromDate);
        //         }
        //         
        //         if (filter.ToDate != null)
        //         {
        //             query = query.Where(x => x.CreatedDate <= filter.ToDate);
        //         }
        //     }
        //
        //     if (sort?.Direction != null && sort.Field != null)
        //     {
        //         switch (sort.Field.Value)
        //         {
        //             case SortCompanyFields.Name:
        //                 query = sort.Direction.Value == SortDirection.Ascending 
        //                     ? query.OrderBy(p => p.Name) 
        //                     : query.OrderByDescending(p => p.Name);
        //                 break;
        //                     
        //             case SortCompanyFields.CreatedDate:
        //                 query = sort.Direction.Value == SortDirection.Ascending 
        //                     ? query.OrderBy(p => p.CreatedDate) 
        //                     : query.OrderByDescending(p => p.CreatedDate);
        //                 break;
        //         }
        //     }
        //
        //     var skip = (pageModel.Page - 1) * pageModel.PageSize;
        //     var take = pageModel.PageSize;
        //
        //     var total = await query.CountAsync();
        //     var items = await query
        //         .Skip(skip)
        //         .Take(take)
        //         .ToListAsync();
        //
        //     return new PageModel<CompanyModel>
        //     {
        //         Page = pageModel.Page,
        //         PageSize = pageModel.PageSize,
        //         Total = total,
        //         Items = items
        //     };
        // }
    }
}