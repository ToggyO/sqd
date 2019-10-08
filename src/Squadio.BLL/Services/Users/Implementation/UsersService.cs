using System;
using System.Threading.Tasks;
using Magora.Passwords;
using Mapper;
using Squadio.BLL.Services.Email;
using Squadio.Common.Models.Email;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Auth;
using Squadio.DTO.Users;

namespace Squadio.BLL.Services.Users.Implementation
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _repository;
        private readonly IEmailService<PasswordSetEmailModel> _passwordSetMailService;
        private readonly IPasswordService _passwordService;
        private readonly IMapper _mapper;
        public UsersService(IUsersRepository repository
            , IEmailService<PasswordSetEmailModel> passwordSetMailService
            , IPasswordService passwordService
            , IMapper mapper
            )
        {
            _repository = repository;
            _passwordSetMailService = passwordSetMailService;
            _passwordService = passwordService;
            _mapper = mapper;
        }

        public async Task SignUp(string email)
        {
            var user = await _repository.GetByEmail(email);
            if(user != null)
                throw new Exception("Email already used");
            
            var code = Guid.NewGuid().ToString("N");

            await _passwordSetMailService.Send(new PasswordSetEmailModel
            {
                Code = code,
                To = email
            });

            user = new UserModel
            {
                Email = email,
                CreatedDate = DateTime.UtcNow
            };

            user = await _repository.Create(user);
            await _repository.AddPasswordRequest(user.Id, code);
        }

        public async Task<UserDTO> SetPassword(string code, string password)
        {
            var userPasswordRequest = await _repository.GetByChangePasswordRequestsCode(code);
            if(userPasswordRequest.IsActivated)
                throw new Exception("Code already used");
            
            var passwordModel = await _passwordService.CreatePassword(password);
            await _repository.SavePassword(userPasswordRequest.UserId, passwordModel.Hash, passwordModel.Salt);
            
            await _repository.ActivateChangePasswordRequestsCode(code);

            var user = await _repository.GetById(userPasswordRequest.UserId);

            var userDTO = _mapper.Map<UserModel, UserDTO>(user);
            return userDTO;
        }
    }
}
