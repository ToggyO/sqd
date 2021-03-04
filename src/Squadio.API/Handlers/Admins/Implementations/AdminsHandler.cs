using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.BLL.Providers.Admin;
using Squadio.BLL.Services.Admin;
using Squadio.BLL.Services.Tokens;
using Squadio.Common.Extensions;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Enums;
using Squadio.DTO.Models.Admin;
using Squadio.DTO.Models.Auth;
using Squadio.DTO.Models.Users;
using Squadio.DTO.Models.Users.Settings;

namespace Squadio.API.Handlers.Admins.Implementations
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
        public async Task<Response<PageModel<UserWithCompaniesDTO>>> GetUsersPage(PageModel model, UserFilterAdminDTO filter)
        {
            var result = await _provider.GetUsersPage(model, filter);
            return result;
        }

        public async Task<Response<UserDetailAdminDTO>> GetUserDetail(Guid userId)
        {
            var result = await _provider.GetUserDetail(userId);
            return result;
        }

        public async Task<Response> BlockUser(Guid userId)
        {
            var result = await _provider.SetUserStatus(userId, UserStatus.Blocked);
            return result;
        }

        public async Task<Response> UnblockUser(Guid userId)
        {
            var result = await _provider.SetUserStatus(userId, UserStatus.Active);
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

        public async Task<Response> ResetPassword(UserResetPasswordDTO dto)
        {
            var result = await _service.ResetPassword(dto.Code, dto.Password);
            return result;
        }

        public async Task<Response<AuthInfoDTO>> Authenticate(CredentialsDTO request)
        {
            var user = await _provider.GetUserDetail(request.Email);
            if (user.Data == null)
            {
                return new SecurityErrorResponse<AuthInfoDTO>()
                {
                    Code = ErrorCodes.Security.AuthDataInvalid,
                    Message = ErrorMessages.Security.AuthDataInvalid,
                };
            }

            if (user.Data.RoleId != RoleGuid.Admin)
            {
                return new SecurityErrorResponse<AuthInfoDTO>()
                {
                    Code = ErrorCodes.Security.AuthDataInvalid,
                    Message = ErrorMessages.Security.AuthDataInvalid,
                };
            }
            var result = await _tokensService.Authenticate(request);
            return result;
        }

        public async Task<Response<TokenDTO>> RefreshToken(string refreshToken)
        {
            var result = await _tokensService.RefreshToken(refreshToken);
            return result;
        }

        public async Task<Response<PageModel<CompanyDetailAdminDTO>>> GetCompanyPage(PageModel model, CompanyFilterAdminDTO filter)
        {
            var result = await _provider.GetCompanyPage(model, filter);
            return result;
        }

        public async Task<Response<CompanyDetailAdminDTO>> GetCompanyDetail(Guid companyId)
        {
            var result = await _provider.GetCompanyDetail(companyId);
            return result;
        }

        public async Task<Response<PageModel<UserWithRoleDTO>>> GetCompanyUsersPage(PageModel model, Guid companyId)
        {
            var result = await _provider.GetCompanyUsersPage(model, companyId);
            return result;
        }
    }
}