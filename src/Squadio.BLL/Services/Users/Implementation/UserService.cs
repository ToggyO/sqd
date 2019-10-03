using Squadio.DAL.Repository.Users;

namespace Squadio.BLL.Services.Users.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }
    }
}
