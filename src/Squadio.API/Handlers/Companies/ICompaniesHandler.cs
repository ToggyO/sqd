using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Companies;
using Squadio.DTO.Pages;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.Companies
{
    public interface ICompaniesHandler
    {
        Task<Response<PageModel<UserDTO>>> GetCompanyUsers(Guid companyId, PageModel model);
        Task<Response<CompanyDTO>> GetCompany(Guid id);
        Task<Response<CompanyDTO>> CreateCompany(CreateCompanyDTO dto, ClaimsPrincipal claims);
    }
}