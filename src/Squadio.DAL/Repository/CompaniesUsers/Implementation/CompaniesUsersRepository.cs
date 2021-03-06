using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Squadio.Common.Models.Pages;
using Squadio.DAL.Extensions;
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

        public async Task<PageModel<CompanyUserModel>> GetCompaniesUsers(PageModel model
            , Guid? userId = null
            , Guid? companyId = null
            , IEnumerable<MembershipStatus> statuses = null)
        {
            IQueryable<CompanyUserModel> query = _context.CompaniesUsers
                .Include(x => x.User).ThenInclude(x => x.Avatar)
                .Include(x => x.Company);

            if (userId.HasValue)
            {
                query = query.Where(x => x.UserId == userId);
            }

            if (companyId.HasValue)
            {
                query = query.Where(x => x.CompanyId == companyId);
            }
            
            if (statuses != null)
            {
                var userStatuses = statuses.ToList();
                if (userStatuses?.Any() == true)
                {
                    query = query.Where(x => userStatuses.Contains(x.Status));
                }
            }

            query = query
                .OrderByDescending(x => x.Status)
                .ThenByDescending(x => x.CreatedDate);

            var total = await query.CountAsync();
            var items = await query
                .GetPage(model)
                .ToListAsync();

            return new PageModel<CompanyUserModel>
            {
                Page = model.Page,
                PageSize = model.PageSize,
                Total = total,
                Items = items
            };
        }

        public async Task<IEnumerable<CompanyUserModel>> GetCompanyUsersByEmails(Guid companyId, IEnumerable<string> emails)
        {
            var query = _context.CompaniesUsers.Where(x => x.CompanyId == companyId);
            
            if (emails != null)
            {
                var userEmails = emails.ToList();
                if (userEmails?.Any() == true)
                {
                    query = query.Where(x => userEmails.Contains(x.User.Email));
                }
            }

            var items = await query
                .ToListAsync();

            return items;
        }

        public async Task<CompanyUserModel> GetCompanyUserByEmail(Guid companyId, string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;
            IQueryable<CompanyUserModel> query = _context.CompaniesUsers
                .Include(x => x.User).ThenInclude(x => x.Avatar)
                .Include(x => x.Company)
                .Where(x => x.CompanyId == companyId);

            return await query.FirstOrDefaultAsync(x=>x.User.Email.ToUpper() == email.ToUpper());
        }

        public async Task<PageModel<CompanyUserModel>> GetCompanyUsersByEmails(PageModel model, Guid companyId, IEnumerable<string> emails)
        {
            IQueryable<CompanyUserModel> query = _context.CompaniesUsers
                .Include(x => x.User).ThenInclude(x => x.Avatar)
                .Include(x => x.Company)
                .Where(x => x.CompanyId == companyId);
            
            if (emails != null)
            {
                var userEmails = emails.ToList();
                if (userEmails?.Any() == true)
                {
                    query = query.Where(x => userEmails.Contains(x.User.Email));
                }
            }
            
            query = query.OrderBy(x => x.User.Email);

            var total = await query.CountAsync();
            var items = await query
                .GetPage(model)
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
                .Include(x => x.Company).ThenInclude(x=>x.Creator).ThenInclude(x=>x.Avatar)
                .Include(x=>x.User).ThenInclude(x=>x.Avatar)
                .Where(x => x.CompanyId == companyId && x.UserId == userId)
                .FirstOrDefaultAsync();
            return item;
        }

        public async Task<CompanyUserModel> AddCompanyUser(Guid companyId, Guid userId, MembershipStatus membershipStatus)
        {
            var item = new CompanyUserModel
            {
                CompanyId = companyId,
                UserId = userId,
                Status = membershipStatus,
                CreatedDate = DateTime.UtcNow
            };
            var entry = await _context.CompaniesUsers.AddAsync(item);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task DeleteCompanyUser(Guid companyId, Guid userId)
        {
            var item = await _context.CompaniesUsers
                .Where(x => x.CompanyId == companyId && x.UserId == userId)
                .FirstOrDefaultAsync();
            _context.CompaniesUsers.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CompanyUserModel>> GetCompaniesByUsers(IEnumerable<Guid> userIds)
        {
            var items = await _context.CompaniesUsers
                .Include(x => x.Company)
                .Where(x => userIds.Contains(x.UserId))
                .ToListAsync();
            return items;
        }

        public async Task<IEnumerable<CompanyUserModel>> GetUsersByCompanies(IEnumerable<Guid> companyIds)
        {
            var items = await _context.CompaniesUsers
                .Include(x => x.User)
                .Where(x => companyIds.Contains(x.CompanyId))
                .ToListAsync();
            return items;
        }

        public async Task DeleteCompanyUsers(Guid companyId, IEnumerable<string> emails)
        {
            var emailsUpper = emails.Select(s => s.ToUpper());

            var query = _context.CompaniesUsers
                .Include(x => x.User)
                .Where(x => x.CompanyId == companyId);
            query = query.Where(x => emailsUpper.Contains(x.User.Email.ToUpper()));

            var items = await query.ToListAsync();
            
            _context.CompaniesUsers.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeCompanyUser(Guid companyId, IEnumerable<Guid> userIds, MembershipStatus membershipStatus)
        {
            var items = userIds.Select(userId => new CompanyUserModel
                    {
                        CompanyId = companyId, 
                        UserId = userId, 
                        Status = membershipStatus, 
                        CreatedDate = DateTime.UtcNow
                    })
                .ToList();

            _context.CompaniesUsers.AddRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task ChangeStatusCompanyUser(Guid companyId, Guid userId, MembershipStatus newMembershipStatus)
        {
            var item = await _context.CompaniesUsers
                .Where(x => x.CompanyId == companyId && x.UserId == userId)
                .FirstOrDefaultAsync();
            item.Status = newMembershipStatus;
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