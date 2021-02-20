using System;
using System.Threading.Tasks;
using Magora.Passwords;
using Mapper;
using Squadio.BLL.Services.Rabbit;
using Squadio.Common.Models.Email;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.ChangePassword;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Users;

namespace Squadio.BLL.Services.Admins.Implementation
{
    public class AdminsService : IAdminsService
    {
        private readonly IUsersRepository _repository;
        private readonly IChangePasswordRequestRepository _changePasswordRepository;
        private readonly IRabbitService _rabbitService;
        private readonly IPasswordService _passwordService;
        private readonly IMapper _mapper;
        public AdminsService(IUsersRepository repository
            , IChangePasswordRequestRepository changePasswordRepository
            , IRabbitService rabbitService
            , IPasswordService passwordService
            , IMapper mapper
        )
        {
            _repository = repository;
            _changePasswordRepository = changePasswordRepository;
            _rabbitService = rabbitService;
            _passwordService = passwordService;
            _mapper = mapper;
        }

        public async Task<Response<UserDTO>> SetPassword(string email, string password)
        {
            var user = await _repository.GetByEmail(email);
            
            var passwordModel = await _passwordService.CreatePassword(password);
            await _repository.SavePassword(user.Id, passwordModel.Hash, passwordModel.Salt);

            var userDTO = _mapper.Map<UserModel, UserDTO>(user);
            return new Response<UserDTO>
            {
                Data = userDTO
            };
        }

        public async Task<Response<UserDTO>> ResetPassword(string code, string password)
        {
            var userPasswordRequest = await _changePasswordRepository.GetRequestByCode(code);
            if (userPasswordRequest == null || userPasswordRequest?.IsDeleted == true)
            {
                return new BusinessConflictErrorResponse<UserDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.PasswordChangeRequestInvalid,
                        Message = ErrorMessages.Business.PasswordChangeRequestInvalid
                    }
                });
            }
            
            var passwordModel = await _passwordService.CreatePassword(password);
            await _repository.SavePassword(userPasswordRequest.UserId, passwordModel.Hash, passwordModel.Salt);
            
            await _changePasswordRepository.ActivateAllRequestsForUser(userPasswordRequest.UserId);

            var user = await _repository.GetById(userPasswordRequest.UserId);

            var userDTO = _mapper.Map<UserModel, UserDTO>(user);
            return new Response<UserDTO>
            {
                Data = userDTO
            };
        }

        public async Task<Response> ResetPasswordRequest(string email)
        {
            var user = await _repository.GetByEmail(email);
            if(user == null)
            {
                return new BusinessConflictErrorResponse(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.UserDoesNotExists,
                        Message = ErrorMessages.Business.UserDoesNotExists,
                        Field = ErrorFields.User.Email
                    }
                });
            }

            var code = Guid.NewGuid().ToString("N");

            
            await _changePasswordRepository.ActivateAllRequestsForUser(user.Id);
            
            await _changePasswordRepository.AddRequest(user.Id, code);

            await _rabbitService.Send(new PasswordRestoreAdminEmailModel
            {
                Code = code,
                To = email
            });
            
            return new Response();
        }
    }
}