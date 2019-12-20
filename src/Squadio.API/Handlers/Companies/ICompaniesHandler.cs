using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Companies;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.Companies
{
    public interface ICompaniesHandler
    {
        Task<Response<PageModel<CompanyDTO>>> GetCompanies(PageModel model, CompanyFilter filter);
        Task<Response<PageModel<UserWithRoleDTO>>> GetCompanyUsers(Guid companyId, PageModel model);
        Task<Response<CompanyDTO>> GetCompany(Guid id);
        Task<Response<CompanyDTO>> CreateCompany(CompanyCreateDTO dto, ClaimsPrincipal claims);
        Task<Response<CompanyDTO>> UpdateCompany(Guid companyId, CompanyUpdateDTO dto, ClaimsPrincipal claims);
        Task<Response> DeleteCompanyUser(Guid companyId, Guid userId, ClaimsPrincipal claims);
    }
}