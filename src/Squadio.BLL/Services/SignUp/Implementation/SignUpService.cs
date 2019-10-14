using System;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Mapper;
using Microsoft.Extensions.Options;
using Squadio.BLL.Services.Email;
using Squadio.BLL.Services.Users;
using Squadio.Common.Exceptions.BusinessLogicExceptions;
using Squadio.Common.Exceptions.SecurityExceptions;
using Squadio.Common.Models.Email;
using Squadio.Common.Settings;
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
        private readonly IOptions<GoogleSettings> _googleSettings;
        private readonly IUsersService _usersService;
        private readonly IMapper _mapper;
        public SignUpService(IUsersRepository repository
            , IEmailService<PasswordSetEmailModel> passwordSetMailService
            , IOptions<GoogleSettings> googleSettings
            , IUsersService usersService
            , IMapper mapper
        )
        {
            _repository = repository;
            _passwordSetMailService = passwordSetMailService;
            _googleSettings = googleSettings;
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

        public async Task<UserDTO> SignUpGoogle(string googleToken)
        {
            var infoFromGoogleToken = await GoogleJsonWebSignature.ValidateAsync(googleToken);
            
            if((string) infoFromGoogleToken.Audience != _googleSettings.Value.ClientId)
                throw new SecurityException("security_error", "Incorrect google token");

            var user = await _repository.GetByEmail(infoFromGoogleToken.Email);
            if(user != null)
                throw new BusinessLogicException("business_conflict", "User already exist");

            user = new UserModel
            {
                Name = $"{infoFromGoogleToken.Name}",
                Email = infoFromGoogleToken.Email,
                CreatedDate = DateTime.Now
            };
            user = await _repository.Create(user);

            await _repository.SetRegistrationStep(user.Id, RegistrationStep.EmailConfirmed);
            
            var code = _usersService.GenerateCode();
            await _passwordSetMailService.Send(new PasswordSetEmailModel
            {
                Code = code,
                To = user.Email
            });
            await _repository.AddPasswordRequest(user.Id, code);
            
            var result = _mapper.Map<UserModel, UserDTO>(user);
            return result;
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