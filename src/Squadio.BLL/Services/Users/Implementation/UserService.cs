using System;
using System.Threading.Tasks;
using Squadio.BLL.Services.Email;
using Squadio.Common.Models.Email;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Models.Users;

namespace Squadio.BLL.Services.Users.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMailService<PasswordSetEmailModel> _passwordSetMailService;
        public UserService(IUserRepository repository
            , IMailService<PasswordSetEmailModel> passwordSetMailService)
        {
            _repository = repository;
            _passwordSetMailService = passwordSetMailService;
        }

        public async Task SignUp(string email)
        {
            var user = await _repository.GetByEmail(email);
            if(user != null)
                throw new Exception("Email already used");
            
            var code = Guid.NewGuid().ToString("N");

            user = new UserModel
            {
                Email = email,
                CreatedDate = DateTime.UtcNow
            };

            user = await _repository.Create(user);
            await _repository.AddPasswordRequest(user.Id, code);

            await _passwordSetMailService.Send(new PasswordSetEmailModel
            {
                Code = code,
                Email = email
            });
        }
    }
}
