using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.Admins.Implementation
{
    public class AdminsRepository : IAdminsRepository
    {
        private readonly SquadioDbContext _context;
        public AdminsRepository(SquadioDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CompanyUserModel>> GetCompanyUser(Guid? userId = null, Guid? companyId = null, IEnumerable<UserStatus> statuses = null)
        {
            IQueryable<CompanyUserModel> query = _context.CompaniesUsers
                .Include(x => x.User)
                .Include(x => x.Company);

            if (statuses != null)
            {
                if (statuses.Any())
                {
                    query = query.Where(x => statuses.Contains(x.Status));
                }
            }

            if (userId.HasValue)
            {
                query = query.Where(x => x.UserId == userId);
            }

            if (companyId.HasValue)
            {
                query = query.Where(x => x.CompanyId == companyId);
            }

            return await query.ToListAsync();
        }

        public async Task<PageModel<UserModel>> GetUsers(PageModel pageModel, string search, UserWithCompaniesFilter filter)
        {
            IQueryable<CompanyUserModel> query = _context.CompaniesUsers
                .Include(x => x.Company)
                .Include(x => x.User);

            if (!string.IsNullOrEmpty(search))
            {
                var searchUpper = search.ToUpper();

                query = query.Where(x => x.User.Name.ToUpper().Contains(searchUpper)
                                         || x.User.Email.ToUpper().Contains(searchUpper)
                                         || x.Company.Name.ToUpper().Contains(searchUpper));
            }

            if (filter != null)
            {
                if (filter.Status != null)
                {
                    query = query.Where(x => x.Status == filter.Status);
                }
            }

            var queryUsers = query
                .Select(x => x.User)
                .Distinct()
                .OrderBy(x => x.Name);

            var skip = (pageModel.Page - 1) * pageModel.PageSize;
            var take = pageModel.PageSize;

            var total = await queryUsers.CountAsync();
            var items = await queryUsers
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return new PageModel<UserModel>
            {
                Page = pageModel.Page,
                PageSize = pageModel.PageSize,
                Total = total,
                Items = items
            };
        }

        public async Task<PageModel<CompanyModel>> GetCompanies(PageModel pageModel, CompaniesFilter filter, string search)
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

        public async Task<int> GetCompanyUsersCount(Guid companyId)
        {
            IQueryable<CompanyUserModel> query = _context.CompaniesUsers;
            query = query.Where(x => x.CompanyId == companyId);
            return await query.CountAsync();
        }
    }
}