using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.BLL.Providers.Admins;
using Squadio.BLL.Services.Admins;
using Squadio.BLL.Services.Tokens;
using Squadio.Common.Extensions;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Enums;
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
        public async Task<Response<PageModel<UserDTO>>> GetUsersPage(PageModel model)
        {
            var result = await _provider.GetUsersPage(model);
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
    }
}