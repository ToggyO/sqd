using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;

namespace Squadio.DAL.Repository.Companies
{
    public interface ICompaniesRepository : IBaseRepository<CompanyModel>
    {
        Task<PageModel<CompanyModel>> GetCompaniesOfUser(Guid userId, PageModel model, UserStatus? status = null);
        Task<PageModel<CompanyModel>> GetCompanies(PageModel pageModel, CompaniesFilter filter, string search);
    }
}