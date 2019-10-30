using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Pages;

namespace Squadio.DAL.Repository.Admins.Implementation
{
    public class AdminsRepository : IAdminsRepository
    {
        private readonly SquadioDbContext _context;
        public AdminsRepository(SquadioDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CompanyUserModel>> GetCompanyUser(Guid? userId = null, Guid? companyId = null, UserStatus? status = null)
        {
            IQueryable<CompanyUserModel> query = _context.CompaniesUsers
                .Include(x => x.User)
                .Include(x => x.Company);

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status);
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

        public async Task<PageModel<UserModel>> GetUsers(PageModel pageModel, string search)
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
    }
}