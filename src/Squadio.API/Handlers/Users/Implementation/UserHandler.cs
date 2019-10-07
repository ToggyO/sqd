using System;
using System.Threading.Tasks;
using Squadio.BLL.Providers.Users;
using Squadio.BLL.Services.Users;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Auth;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.Users.Implementation
{
    public class UserHandler : IUserHandler
    {
        private readonly IUserProvider _provider;
        private readonly IUserService _service;
        public UserHandler(IUserProvider provider
            , IUserService service)
        {
            _service = service;
            _provider = provider;
        }
        
        public async Task<Response<UserDTO>> GetById(Guid id)
        {
            var user = await _provider.GetById(id);
            var result = new Response<UserDTO>()
            {
                Data = user
            };
            return result;
        }
        
        public async Task<Response<UserDTO>> GetByCode(string code)
        {
            var user = await _provider.GetByCode(code);
            var result = new Response<UserDTO>()
            {
                Data = user
            };
            return result;
        }

        public async Task<Response<AuthInfoDTO>> SetPassword(UserSetPasswordDTO dto)
        {
            var item = await _service.SetPassword(dto.Code, dto.Password);
            var result = new Response<AuthInfoDTO>
            {
                Data = item
            };
            return result;
        }

        public async Task<Response> SignUp(string email)
        {
            await _service.SignUp(email);
            var result = new Response();
            return result;
        }
    }
}
