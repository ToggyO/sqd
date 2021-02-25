using System;
using System.Threading.Tasks;
using AutoMapper;
using Magora.Passwords;
using Squadio.BLL.Services.Notifications.Emails;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.ChangePassword;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Models.Users;

namespace Squadio.BLL.Services.Admins.Implementations
{
    public class AdminsService : IAdminsService
    {
        private readonly IUsersRepository _repository;
        private readonly IChangePasswordRequestRepository _changePasswordRepository;
        private readonly IPasswordService _passwordService;
        private readonly IEmailNotificationsService _emailNotificationsService;
        private readonly IMapper _mapper;

        public AdminsService(IUsersRepository repository
            , IChangePasswordRequestRepository changePasswordRepository
            , IPasswordService passwordService
            , IEmailNotificationsService emailNotificationsService
            , IMapper mapper
        )
        {
            _repository = repository;
            _changePasswordRepository = changePasswordRepository;
            _passwordService = passwordService;
            _emailNotificationsService = emailNotificationsService;
            _mapper = mapper;
        }

        public async Task<Response> SetPassword(string email, string password)
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

        public async Task<Response> ResetPassword(string code, string password)
        {
            var userPasswordRequest = await _changePasswordRepository.GetRequestByCode(code);
            if (userPasswordRequest == null || userPasswordRequest?.IsDeleted == true)
                return new BusinessConflictErrorResponse(new[]
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.PasswordChangeRequestInvalid,
                        Message = ErrorMessages.Business.PasswordChangeRequestInvalid
                    }
                });

            if (userPasswordRequest.User.RoleId != RoleGuid.Admin)
            {
                return new ForbiddenErrorResponse();
            }

            var passwordModel = await _passwordService.CreatePassword(password);
            await _repository.SavePassword(userPasswordRequest.UserId, passwordModel.Hash, passwordModel.Salt);

            await _changePasswordRepository.ActivateAllRequestsForUser(userPasswordRequest.UserId);
            return new Response();
        }

        public async Task<Response> ResetPasswordRequest(string email)
        {
            var user = await _repository.GetByEmail(email);
            if (user == null)
            {
                return new Response();
                // return new BusinessConflictErrorResponse(new[]
                // {
                //     new Error
                //     {
                //         Code = ErrorCodes.Business.UserDoesNotExists,
                //         Message = ErrorMessages.Business.UserDoesNotExists,
                //         Field = ErrorFields.User.Email
                //     }
                // });
            }

            if (user.RoleId != RoleGuid.Admin)
            {
                return new Response();
                // return new ForbiddenErrorResponse();
            }

            var code = Guid.NewGuid().ToString("N");

            await _changePasswordRepository.ActivateAllRequestsForUser(user.Id);

            await _changePasswordRepository.AddRequest(user.Id, code);

            return await _emailNotificationsService.SendResetPasswordEmail(user.Email, code);
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