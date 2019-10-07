using System;
using System.Threading.Tasks;
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
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IEmailService<PasswordSetEmailModel> _passwordSetMailService;
        private readonly IMapper _mapper;
        public UserService(IUserRepository repository
            , IEmailService<PasswordSetEmailModel> passwordSetMailService
            , IMapper mapper
            )
        {
            _repository = repository;
            _passwordSetMailService = passwordSetMailService;
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

        public async Task<AuthInfoDTO> SetPassword(string code, string password)
        {
            var user = await _repository.GetByCode(code);
            var userDTO = _mapper.Map<UserModel, UserDTO>(user);
            var result = new AuthInfoDTO
            {
                User = userDTO
            };
            return result;
        }
    }
}
