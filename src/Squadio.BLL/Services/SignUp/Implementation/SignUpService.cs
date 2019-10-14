using System;
using System.Threading.Tasks;
using Mapper;
using Squadio.BLL.Services.Email;
using Squadio.BLL.Services.Users;
using Squadio.Common.Models.Email;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Users;

namespace Squadio.BLL.Services.SignUp.Implementation
{
    public class SignUpService : ISignUpService
    {
        private readonly IUsersRepository _repository;
        private readonly IEmailService<PasswordSetEmailModel> _passwordSetMailService;
        private readonly IUsersService _usersService;
        private readonly IMapper _mapper;
        public SignUpService(IUsersRepository repository
            , IEmailService<PasswordSetEmailModel> passwordSetMailService
            , IUsersService usersService
            , IMapper mapper
        )
        {
            _repository = repository;
            _passwordSetMailService = passwordSetMailService;
            _usersService = usersService;
            _mapper = mapper;
        }

        public async Task SignUp(string email)
        {
            var user = await _repository.GetByEmail(email);
            if(user != null)
                throw new Exception("Email already used");
            
            var code = _usersService.GenerateCode();

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

            await _repository.SetRegistrationStep(user.Id, RegistrationStep.New);
        }

        public async Task<UserDTO> SignUpPassword(string email, string code, string password)
        {
            var user = await _usersService.SetPassword(email, code, password);

            await _repository.SetRegistrationStep(user.Id, RegistrationStep.PasswordEntered);

            return user;
        }

        public async Task<UserDTO> SignUpUsername(Guid id, UserUpdateDTO updateDTO)
        {
            var user = await _usersService.UpdateUser(id, updateDTO);

            await _repository.SetRegistrationStep(user.Id, RegistrationStep.Done);

            return user;
        }
    }
}