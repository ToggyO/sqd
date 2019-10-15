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

        public async Task<Response<UserDTO>> SetPassword(string email, string code, string password)
        {
            var userPasswordRequest = await _repository.GetChangePasswordRequests(email, code);
            if (userPasswordRequest.IsActivated)
            {
                return new ErrorResponse<UserDTO>
                {
                    Code = ErrorCodes.Business.PasswordChangeRequestInvalid,
                    Message = ErrorMessages.Business.PasswordChangeCodeInvalid,
                    HttpStatusCode = HttpStatusCode.Conflict
                };
            }
            
            var passwordModel = await _passwordService.CreatePassword(password);
            await _repository.SavePassword(userPasswordRequest.UserId, passwordModel.Hash, passwordModel.Salt);
            
            await _repository.ActivateChangePasswordRequestsCode(userPasswordRequest.UserId, code);

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
                return new ErrorResponse
                {
                    Code = ErrorCodes.Common.NotFound,
                    Message = ErrorMessages.Common.NotFound,
                    // TODO: find correct http code for this
                    HttpStatusCode = HttpStatusCode.Conflict,
                    Errors = new List<Error>
                    {
                        new Error
                        {
                            Code = ErrorCodes.Business.UserDoesNotExists,
                            Message = ErrorMessages.Business.UserDoesNotExists,
                            Field = ErrorFields.User.Email
                        }
                    }
                };
            }
            
            var code = GenerateCode();
            
            await _repository.AddChangePasswordRequest(user.Id, code);

            await _passwordResetMailService.Send(new PasswordResetEmailModel
            {
                Code = code,
                To = email
            });
            
            return new Response();
        }

        public async Task<Response<UserDTO>> UpdateUser(Guid id, UserUpdateDTO updateDTO)
        {
            var userEntity = await _repository.GetById(id);
            if (userEntity == null)
            {
                return new ErrorResponse<UserDTO>
                {
                    Code = ErrorCodes.Common.NotFound,
                    Message = ErrorMessages.Common.NotFound,
                    // TODO: find correct http code for this
                    HttpStatusCode = HttpStatusCode.Conflict,
                    Errors = new List<Error>
                    {
                        new Error
                        {
                            Code = ErrorCodes.Business.UserDoesNotExists,
                            Message = ErrorMessages.Business.UserDoesNotExists,
                            Field = ErrorFields.User.Id
                        }
                    }
                };
            }

            userEntity.Name = updateDTO.Name;
            userEntity = await _repository.Update(userEntity);
            
            var result = _mapper.Map<UserModel, UserDTO>(userEntity);
            return new Response<UserDTO>
            {
                Data = result
            };
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
