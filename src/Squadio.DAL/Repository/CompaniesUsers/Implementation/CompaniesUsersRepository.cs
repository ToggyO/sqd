using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Common.Models.Pages;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.CompaniesUsers.Implementation
{
    public class CompaniesUsersRepository : ICompaniesUsersRepository
    {
        private readonly SquadioDbContext _context;
        public CompaniesUsersRepository(SquadioDbContext context)
        {
            _context = context;
        }

        public async Task<PageModel<CompanyUserModel>> GetCompanyUsers(Guid companyId, PageModel model)
        {
            IQueryable<CompanyUserModel> query = _context.CompaniesUsers
                .Include(x => x.User)
                .Include(x => x.Company)
                .Where(x => x.CompanyId == companyId);
            
            var skip = (model.Page - 1) * model.PageSize;
            var take = model.PageSize;

            var total = await query.CountAsync();
            var items = await query
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return new PageModel<CompanyUserModel>
            {
                Page = model.Page,
                PageSize = model.PageSize,
                Total = total,
                Items = items
            };
        }
        
        public async Task<PageModel<CompanyUserModel>> GetUserCompanies(Guid userId, PageModel model)
        {
            IQueryable<CompanyUserModel> query = _context.CompaniesUsers
                .Include(x => x.User)
                .Include(x => x.Company)
                .Where(x => x.UserId == userId);
            
            var skip = (model.Page - 1) * model.PageSize;
            var take = model.PageSize;

            var total = await query.CountAsync();
            var items = await query
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return new PageModel<CompanyUserModel>
            {
                Page = model.Page,
                PageSize = model.PageSize,
                Total = total,
                Items = items
            };
        }

        public async Task<CompanyUserModel> GetCompanyUser(Guid companyId, Guid userId)
        {
            var item = await _context.CompaniesUsers
                .Include(x => x.Company)
                .Include(x => x.User)
                .Where(x => x.CompanyId == companyId && x.UserId == userId)
                .FirstOrDefaultAsync();
            return item;
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

        public async Task AddCompanyUser(Guid companyId, Guid userId, UserStatus userStatus)
        {
            var item = new CompanyUserModel
            {
                CompanyId = companyId,
                UserId = userId,
                Status = userStatus,
                CreatedDate = DateTime.UtcNow
            };
            _context.CompaniesUsers.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCompanyUser(Guid companyId, Guid userId)
        {
            var item = await _context.CompaniesUsers
                .Where(x => x.CompanyId == companyId && x.UserId == userId)
                .FirstOrDefaultAsync();
            _context.CompaniesUsers.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeCompanyUser(Guid companyId, IEnumerable<Guid> userIds, UserStatus userStatus)
        {
            var items = userIds.Select(userId => new CompanyUserModel
                    {
                        CompanyId = companyId, 
                        UserId = userId, 
                        Status = userStatus, 
                        CreatedDate = DateTime.UtcNow
                    })
                .ToList();

            _context.CompaniesUsers.AddRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task ChangeStatusCompanyUser(Guid companyId, Guid userId, UserStatus newUserStatus)
        {
            var item = await _context.CompaniesUsers
                .Where(x => x.CompanyId == companyId && x.UserId == userId)
                .FirstOrDefaultAsync();
            item.Status = newUserStatus;
            _context.CompaniesUsers.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetCompanyUsersCount(Guid companyId)
        {
            IQueryable<CompanyUserModel> query = _context.CompaniesUsers;
            query = query.Where(x => x.CompanyId == companyId);
            return await query.CountAsync();
        }
    }
}