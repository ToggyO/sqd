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

        public async Task<UserModel> GetUserById(Guid userId)
        {
            var entity = await _context.Users
                .Include(x => x.Role)
                .Include(x => x.Avatar)
                .FirstOrDefaultAsync(x => x.Id == userId);
            return entity;
        }
    }
}