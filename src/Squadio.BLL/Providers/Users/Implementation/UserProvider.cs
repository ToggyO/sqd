using Squadio.DAL.Repository.Users;

namespace Squadio.BLL.Providers.Users.Implementation
{
    public class UserProvider : IUserProvider
    {
        private readonly IUserRepository _repository;
        public UserProvider(IUserRepository repository)
        {
            _repository = repository;
        }
    }
}
