using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.BLL.Providers.Companies;
using Squadio.BLL.Services.Companies;
using Squadio.Common.Exceptions.PermissionException;
using Squadio.Common.Extensions;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Companies;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.Companies.Implementation
{
    public class CompaniesHandler : ICompaniesHandler
    {
        private readonly ICompaniesProvider _provider;
        private readonly ICompaniesService _service;
        
        public CompaniesHandler(ICompaniesProvider provider
            , ICompaniesService service)
        {
            _provider = provider;
            _service = service;
        }

        public async Task<Response<CompanyDTO>> GetCompany(Guid id)
        {
            var result = await _provider.GetById(id);
            return result;
        }

        public async Task<Response<CompanyDTO>> CreateCompany(CreateCompanyDTO dto, ClaimsPrincipal claims)
        {
            var result = await _service.Create(claims.GetUserId() ?? throw new PermissionException(), dto);
            return result;
        }
    }
}