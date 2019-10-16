using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.BLL.Providers.Users;
using Squadio.BLL.Services.Users;
using Squadio.Common.Extensions;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Pages;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.Users.Implementation
{
    public class UsersHandler : IUsersHandler
    {
        private readonly IUsersProvider _provider;
        private readonly IUsersService _service;
        public UsersHandler(IUsersProvider provider
            , IUsersService service)
        {
            _service = service;
            _provider = provider;
        }

        public async Task<Response<PageModel<UserDTO>>> GetPage(PageModel model)
        {
            var result = await _provider.GetPage(model);
            return result;
        }

        public async Task<Response<UserDTO>> GetCurrentUser(ClaimsPrincipal claims)
        {
            var result = await _provider.GetById(claims.GetUserId());
            return result;
        }

        public async Task<Response<UserDTO>> UpdateCurrentUser(UserUpdateDTO dto, ClaimsPrincipal claims)
        {
            var result = await _service.UpdateUser(claims.GetUserId(), dto);
            return result;
        }

        public async Task<Response<UserDTO>> GetById(Guid id)
        {
            var result = await _provider.GetById(id);
            return result;
        }

        public async Task<Response> ResetPasswordRequest(string email)
        {
            await _service.ResetPasswordRequest(email);
            var result = new Response();
            return result;
        }

        public async Task<Response<UserDTO>> SetPassword(UserSetPasswordDTO dto)
        {
            var result = await _service.SetPassword(dto.Email, dto.Code, dto.Password);
            return result;
        }
    }
}
