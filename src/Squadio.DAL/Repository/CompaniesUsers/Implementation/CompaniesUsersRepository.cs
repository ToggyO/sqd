using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;

namespace Squadio.DAL.Repository.CompaniesUsers.Implementation
{
    public class CompaniesUsersRepository : ICompaniesUsersRepository
    {
        private readonly SquadioDbContext _context;
        public CompaniesUsersRepository(SquadioDbContext context)
        {
            _context = context;
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