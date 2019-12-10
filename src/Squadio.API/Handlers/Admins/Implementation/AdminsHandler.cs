using System;
using System.Threading.Tasks;
using Squadio.BLL.Providers.Admins;
using Squadio.BLL.Services.Admins;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Companies;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.Admins.Implementation
{
    public class AdminsHandler : IAdminsHandler
    {
        private readonly IAdminsProvider _provider;
        private readonly IAdminsService _service;

        public AdminsHandler(IAdminsProvider provider
            , IAdminsService service)
        {
            _provider = provider;
            _service = service;
        }
        public async Task<Response<PageModel<UserWithCompaniesDTO>>> GetUsersPage(PageModel model, string search, UserWithCompaniesFilter filter)
        {
            var result = await _provider.GetUsersPage(model, search, filter);
            return result;
        }

        public async Task<Response<PageModel<CompanyListDTO>>> GetCompaniesPage(PageModel model, CompaniesFilter filter, string search)
        {
            var result = await _provider.GetCompaniesPage(model, filter, search);
            return result;
        }

        public async Task<Response<CompanyDetailDTO>> GetCompanyDetail(Guid companyId)
        {
            var result = await _provider.GetCompanyDetail(companyId);
            return result;
        }
    }
}