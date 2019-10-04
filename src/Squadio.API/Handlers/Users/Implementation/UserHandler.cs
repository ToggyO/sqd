using System;
using System.Threading.Tasks;
using Squadio.BLL.Providers.Users;
using Squadio.BLL.Services.Users;
using Squadio.Domain.Models.Users;

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
        public async Task<UserModel> GetById(Guid id)
        {
            var user = await _provider.GetById(id);
            return user;
        }
    }
}
