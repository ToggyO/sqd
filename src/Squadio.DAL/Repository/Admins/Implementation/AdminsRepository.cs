using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;
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
            IQueryable<CompanyUserModel> query = _context.CompaniesUsers;

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

            query
                .Include(x => x.Company)
                .Include(x => x.User);

            return await query.ToListAsync();
        }
    }
}