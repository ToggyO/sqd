using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.BLL.Providers.Companies;
using Squadio.BLL.Services.Companies;
using Squadio.BLL.Services.Membership;
using Squadio.Common.Extensions;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Companies;
using Squadio.DTO.Invites;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.Companies.Implementation
{
    public class CompaniesHandler : ICompaniesHandler
    {
        private readonly ICompaniesProvider _provider;
        private readonly ICompaniesService _service;
        private readonly IMembershipService _membershipService;
        
        public CompaniesHandler(ICompaniesProvider provider
            , ICompaniesService service
            , IMembershipService membershipService)
        {
            _provider = provider;
            _service = service;
            _membershipService = membershipService;
        }

        public async Task<Response<PageModel<CompanyDTO>>> GetCompanies(PageModel model)
        {
            var result = await _provider.GetCompanies(model);
            return result;
        }

        public async Task<Response<PageModel<UserWithRoleDTO>>> GetCompanyUsers(Guid companyId, PageModel model)
        {
            var result = await _provider.GetCompanyUsers(companyId, model);
            return result;
        }

        public async Task<Response<CompanyDTO>> GetCompany(Guid id)
        {
            var result = await _provider.GetById(id);
            return result;
        }

        public async Task<Response<CompanyDTO>> CreateCompany(CompanyCreateDTO dto, ClaimsPrincipal claims)
        {
            var result = await _service.Create(claims.GetUserId(), dto);
            return result;
        }

        public async Task<Response<CompanyDTO>> UpdateCompany(Guid companyId, CompanyUpdateDTO dto, ClaimsPrincipal claims)
        {
            var result = await _service.Update(companyId, claims.GetUserId(), dto);
            return result;
        }

        public async Task<Response> DeleteCompanyUser(Guid companyId, Guid userId, ClaimsPrincipal claims)
        {
            var result = await _membershipService.DeleteUserFromCompany(companyId, userId, claims.GetUserId());
            return result;
        }

        public async Task<Response> InviteCompanyUsers(Guid companyId, CreateInvitesDTO dto, ClaimsPrincipal claims)
        {
            var result = await _membershipService.InviteUsersToCompany(companyId, claims.GetUserId(), dto);
            return result;
        }
    }
}