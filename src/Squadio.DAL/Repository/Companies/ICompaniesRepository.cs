using System;
using System.Threading.Tasks;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;
using Squadio.DTO.Pages;

namespace Squadio.DAL.Repository.Companies
{
    public interface ICompaniesRepository : IBaseRepository<CompanyModel>
    {
        Task<PageModel<CompanyModel>> GetCompaniesOfUser(Guid userId, PageModel model, UserStatus? status = null);
    }
}