using System;
using System.Threading.Tasks;
using Magora.Passwords;
using Mapper;
using Squadio.BLL.Services.Email;
using Squadio.Common.Exceptions.BusinessLogicExceptions;
using Squadio.Common.Models.Email;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Users;

namespace Squadio.BLL.Services.Users.Implementation
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _repository;
        private readonly IEmailService<PasswordSetEmailModel> _passwordSetMailService;
        private readonly IEmailService<PasswordResetEmailModel> _passwordResetMailService;
        private readonly IPasswordService _passwordService;
        private readonly IMapper _mapper;
        public UsersService(IUsersRepository repository
            , IEmailService<PasswordSetEmailModel> passwordSetMailService
            , IEmailService<PasswordResetEmailModel> passwordResetMailService
            , IPasswordService passwordService
            , IMapper mapper
            )
        {
            _repository = repository;
            _passwordSetMailService = passwordSetMailService;
            _passwordResetMailService = passwordResetMailService;
            _passwordService = passwordService;
            _mapper = mapper;
        }

        public async Task<UserDTO> SetPassword(string email, string code, string password)
        {
            var userPasswordRequest = await _repository.GetChangePasswordRequests(email, code);
            if(userPasswordRequest.IsActivated)
                throw new Exception("Code already used");
            
            var passwordModel = await _passwordService.CreatePassword(password);
            await _repository.SavePassword(userPasswordRequest.UserId, passwordModel.Hash, passwordModel.Salt);
            
            await _repository.ActivateChangePasswordRequestsCode(code);

            var user = await _repository.GetById(userPasswordRequest.UserId);

            var userDTO = _mapper.Map<UserModel, UserDTO>(user);
            return userDTO;
        }

        public async Task ResetPasswordRequest(string email)
        {
            var user = await _repository.GetByEmail(email);
            if(user == null)
                throw new Exception("User not found");
            
            var code = GenerateCode();
            
            await _repository.AddPasswordRequest(user.Id, code);

            await _passwordResetMailService.Send(new PasswordResetEmailModel
            {
                Code = code,
                To = email
            });
        }

        public async Task<UserDTO> UpdateUser(Guid id, UserUpdateDTO updateDTO)
        {
            var userEntity = await _repository.GetById(id);
            if(userEntity == null) 
                throw new BusinessLogicException("","User not found","userId");
            
            userEntity.Name = updateDTO.Name;
            userEntity = await _repository.Update(userEntity);
            
            var result = _mapper.Map<UserModel, UserDTO>(userEntity);
            return result;
        }

        public string GenerateCode(int length = 6)
        {
            var generator = new Random();
            
            var result = "";
            
            while (result.Length < length)
            {
                result += generator.Next(0, 9);
            }
            
            return result;
        }
    }
}
