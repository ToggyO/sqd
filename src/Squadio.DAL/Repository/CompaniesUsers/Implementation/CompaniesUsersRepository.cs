using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Pages;

namespace Squadio.DAL.Repository.CompaniesUsers.Implementation
{
    public class CompaniesUsersRepository : ICompaniesUsersRepository
    {
        private readonly SquadioDbContext _context;
        public CompaniesUsersRepository(SquadioDbContext context)
        {
            _context = context;
        }

        public async Task<PageModel<UserModel>> GetCompanyUsers(Guid companyId, PageModel model)
        {
            var query = _context.CompaniesUsers
                .Include(x => x.User)
                .Where(x => x.CompanyId == companyId);
            
            var total = await query.CountAsync();
            var items = await query
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize)
                .Select(x => x.User)
                .ToListAsync();
            
            var result = new PageModel<UserModel>
            {
                Page = model.Page,
                PageSize = model.PageSize,
                Total = total,
                Items = items
            };
            return result;
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
    }
}