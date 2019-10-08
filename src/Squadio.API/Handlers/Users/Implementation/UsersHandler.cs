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
        
        public async Task<Response<UserDTO>> GetById(Guid id)
        {
            var user = await _provider.GetById(id);
            var result = new Response<UserDTO>()
            {
                Data = user
            };
            return result;
        }

        public async Task<Response> SignUp(string email)
        {
            await _service.SignUp(email);
            var result = new Response();
            return result;
        }
        
        public async Task<Response<UserDTO>> SetPassword(UserSetPasswordDTO dto)
        {
            var item = await _service.SetPassword(dto.Code, dto.Password);
            var result = new Response<UserDTO>
            {
                Data = item
            };
            return result;
        }
    }
}
