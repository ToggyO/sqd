using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Enums;
using Squadio.DTO.Companies;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.Companies
{
    public interface ICompaniesProvider
    {
        Task<Response<PageModel<CompanyDTO>>> GetCompanies(PageModel model);
        Task<Response<PageModel<CompanyUserDTO>>> GetUserCompanies(Guid userId, PageModel model);
        Task<Response<PageModel<CompanyUserDTO>>> GetCompanyUsers(Guid companyId, PageModel model);
        Task<Response<CompanyDTO>> GetById(Guid id);
    }
}