﻿using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.BLL.Providers.Admins;
using Squadio.BLL.Services.Admins;
using Squadio.BLL.Services.Tokens;
using Squadio.Common.Extensions;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Auth;
using Squadio.DTO.Companies;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.Admins.Implementation
{
    public class AdminsHandler : IAdminsHandler
    {
        private readonly IAdminsProvider _provider;
        private readonly IAdminsService _service;
        private readonly ITokensService _tokensService;

        public AdminsHandler(IAdminsProvider provider
            , IAdminsService service
            , ITokensService tokensService)
        {
            _provider = provider;
            _service = service;
            _tokensService = tokensService;
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

        public async Task<Response> ChangePassword(UserSetPasswordDTO dto, ClaimsPrincipal claims)
        {
            var userResponse = await _provider.GetUserDetail(claims.GetUserId());
            if (!userResponse.IsSuccess)
                return userResponse;
            
            var oldPasswordCorrectResponse = await _tokensService.Authenticate(new CredentialsDTO
            {
                Email = userResponse.Data.Email,
                Password = dto.OldPassword
            });

            if (!oldPasswordCorrectResponse.IsSuccess)
                return oldPasswordCorrectResponse;

            return await _service.SetPassword(userResponse.Data.Email, dto.Password);
        }

        public async Task<Response> ResetPasswordRequest(string email)
        {
            await _service.ResetPasswordRequest(email);
            var result = new Response();
            return result;
        }

        public async Task<Response<UserDTO>> ResetPassword(UserResetPasswordDTO dto)
        {
            var result = await _service.ResetPassword(dto.Code, dto.Password);
            return result;
        }
    }
}