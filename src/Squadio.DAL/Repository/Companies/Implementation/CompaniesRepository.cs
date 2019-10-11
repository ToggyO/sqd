using System;
using System.Threading.Tasks;
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
            _context.Update(company);
            await _context.SaveChangesAsync();
            return company;
        }
    }
}