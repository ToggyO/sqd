using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Companies;

namespace Squadio.API.Handlers.Companies
{
    public interface ICompaniesHandler
    {
        Task<Response<CompanyDTO>> GetCompany(Guid id);
        Task<Response<CompanyDTO>> CreateCompany(CreateCompanyDTO dto, ClaimsPrincipal claims);
    }
}