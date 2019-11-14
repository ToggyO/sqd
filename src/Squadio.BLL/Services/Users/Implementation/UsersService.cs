using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Magora.Passwords;
using Mapper;
using Squadio.BLL.Services.Email;
using Squadio.Common.Models.Email;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.ChangePassword;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Users;

namespace Squadio.BLL.Services.Users.Implementation
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _repository;
        private readonly IChangePasswordRequestRepository _changePasswordRepository;
        private readonly IEmailService<PasswordRestoreEmailModel> _passwordRestoreMailService;
        private readonly IPasswordService _passwordService;
        private readonly IMapper _mapper;
        public UsersService(IUsersRepository repository
            , IChangePasswordRequestRepository changePasswordRepository
            , IEmailService<PasswordRestoreEmailModel> passwordRestoreMailService
            , IPasswordService passwordService
            , IMapper mapper
            )
        {
            _repository = repository;
            _changePasswordRepository = changePasswordRepository;
            _passwordRestoreMailService = passwordRestoreMailService;
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

        public async Task<Response<UserDTO>> SetPasswordUsingCode(string code, string password)
        {
            var userPasswordRequest = await _changePasswordRepository.GetRequestByCode(code);
            if (userPasswordRequest == null || userPasswordRequest?.IsActivated == true)
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

            await _passwordRestoreMailService.Send(new PasswordRestoreEmailModel
            {
                Code = code,
                To = email
            });
            
            return new Response();
        }

        public async Task<Response<UserDTO>> UpdateUser(Guid id, UserUpdateDTO dto)
        {
            var userEntity = await _repository.GetById(id);
            if (userEntity == null)
            {
                return new BusinessConflictErrorResponse<UserDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.UserDoesNotExists,
                        Message = ErrorMessages.Business.UserDoesNotExists,
                        Field = ErrorFields.User.Id
                    }
                });
            }

            userEntity.Name = dto.Name;
            userEntity = await _repository.Update(userEntity);
            
            var result = _mapper.Map<UserModel, UserDTO>(userEntity);
            return new Response<UserDTO>
            {
                Data = result
            };
        }

        public async Task<Response<UserDTO>> DeleteUser(Guid id)
        {
            var resultEntity = await _repository.Delete(id);
            
            var result = _mapper.Map<UserModel, UserDTO>(resultEntity);
            
            return new Response<UserDTO>
            {
                Data = result
            };
        }

        public Task<Response> ChangeEmailRequest(Guid id, ChangeEmailDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
