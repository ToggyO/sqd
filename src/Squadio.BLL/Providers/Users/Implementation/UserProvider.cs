using System;
using System.Threading.Tasks;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Models.Users;

namespace Squadio.BLL.Providers.Users.Implementation
{
    public class UserProvider : IUserProvider
    {
        private readonly IUserRepository _repository;
        public UserProvider(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserModel> GetById(Guid id)
        {
            var user = await _repository.GetById(id);
            return user;
        }
    }
}
