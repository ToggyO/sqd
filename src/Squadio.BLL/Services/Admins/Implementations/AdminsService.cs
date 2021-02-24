using System;
using System.Threading.Tasks;
using AutoMapper;
using Magora.Passwords;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.ChangePassword;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Users;

namespace Squadio.BLL.Services.Admins.Implementations
{
    public class AdminsService : IAdminsService
    {
        private readonly IChangePasswordRequestRepository _changePasswordRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordService _passwordService;
        private readonly IUsersRepository _repository;

        public AdminsService(IUsersRepository repository
            , IChangePasswordRequestRepository changePasswordRepository
            , IPasswordService passwordService
            , IMapper mapper
        )
        {
            _repository = repository;
            _changePasswordRepository = changePasswordRepository;
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
                return new BusinessConflictErrorResponse<UserDTO>(new[]
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.PasswordChangeRequestInvalid,
                        Message = ErrorMessages.Business.PasswordChangeRequestInvalid
                    }
                });

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
            if (user == null)
                return new BusinessConflictErrorResponse(new[]
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.UserDoesNotExists,
                        Message = ErrorMessages.Business.UserDoesNotExists,
                        Field = ErrorFields.User.Email
                    }
                });

            var code = Guid.NewGuid().ToString("N");


            await _changePasswordRepository.ActivateAllRequestsForUser(user.Id);

            await _changePasswordRepository.AddRequest(user.Id, code);

            return new Response();
        }

        public async Task<Response> CreateAdmin(string email, string name, string password)
        {
            var passwordModel = await _passwordService.CreatePassword(password);
            
            var entity = new UserModel
            {
                Name = name,
                RoleId = RoleGuid.Admin,
                Email = email,
                CreatedDate = DateTime.UtcNow,
                Status = UserStatus.Active,
                IsDeleted = false,
                Hash = passwordModel.Hash,
                Salt = passwordModel.Salt
            };

            await _repository.Create(entity);
            
            return new Response();
        }
    }
}