using System;
using System.Threading.Tasks;
using Magora.Passwords;
using Mapper;
using Squadio.BLL.Providers.Codes;
using Squadio.BLL.Services.Rabbit;
using Squadio.Common.Models.Email;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.ChangeEmail;
using Squadio.DAL.Repository.ChangePassword;
using Squadio.DAL.Repository.SignUp;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Auth;
using Squadio.DTO.Users;

namespace Squadio.BLL.Services.Users.Implementation
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _repository;
        private readonly ISignUpRepository _signUpRepository;
        private readonly ICodeProvider _codeProvider;
        private readonly IChangePasswordRequestRepository _changePasswordRepository;
        private readonly IChangeEmailRequestRepository _changeEmailRepository;
        private readonly IRabbitService _rabbitService;
        private readonly IPasswordService _passwordService;
        private readonly IMapper _mapper;
        public UsersService(IUsersRepository repository
            , ISignUpRepository signUpRepository
            , ICodeProvider codeProvider
            , IChangePasswordRequestRepository changePasswordRepository
            , IChangeEmailRequestRepository changeEmailRepository
            , IRabbitService rabbitService
            , IPasswordService passwordService
            , IMapper mapper
            )
        {
            _repository = repository;
            _signUpRepository = signUpRepository;
            _codeProvider = codeProvider;
            _changePasswordRepository = changePasswordRepository;
            _changeEmailRepository = changeEmailRepository;
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

            await _rabbitService.Send(new PasswordRestoreEmailModel
            {
                Code = code,
                To = email
            });
            
            return new Response();
        }

        public async Task<Response<UserDTO>> CreateUser(UserCreateDTO dto)
        {
            var entity = new UserModel
            {
                Name = dto.Name,
                RoleId = RoleGuid.User,
                Email = dto.Email,
                CreatedDate = DateTime.UtcNow
            };
            
            entity = await _repository.Create(entity);

            await _signUpRepository.SetRegistrationStep(entity.Id, dto.Step, dto.Status);
            
            var result = _mapper.Map<UserModel, UserDTO>(entity);
            
            return new Response<UserDTO>
            {
                Data = result
            };
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

        public async Task<Response> ChangeEmailRequest(Guid id, string newEmail)
        {
            var user = await _repository.GetById(id);

            var checkEmail = await _repository.GetByEmail(newEmail);
            if (checkEmail != null)
            {
                return new BusinessConflictErrorResponse(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.EmailExists,
                        Message = ErrorCodes.Business.EmailExists
                    }
                });
            }

            await _changeEmailRepository.ActivateAllRequestsForUser(user.Id);

            var code = _codeProvider.GenerateNumberCode();

            await _rabbitService.Send(new UserConfirmEmailModel
            {
               Code = code,
               To = newEmail
            });

            await _changeEmailRepository.AddRequest(user.Id, code, newEmail);

            return new Response();
        }

        public async Task<Response<UserDTO>> SetEmail(Guid id, string code)
        {
            var user = await _repository.GetById(id);
            var request = await _changeEmailRepository.GetRequest(user.Id, code);

            if (request == null || request?.IsActivated == true)
            {
                return new PermissionDeniedErrorResponse<UserDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.ConfirmationCodeInvalid,
                        Message = ErrorMessages.Security.ConfirmationCodeInvalid
                    }
                });
            }

            var checkEmail = await _repository.GetByEmail(request.NewEmail);
            if (checkEmail != null)
            {
                return new BusinessConflictErrorResponse<UserDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.EmailExists,
                        Message = ErrorCodes.Business.EmailExists
                    }
                });
            }
            
            // TODO: Check lifetime of request if needed

            await _changeEmailRepository.ActivateAllRequestsForUser(user.Id);

            user.Email = request.NewEmail;

            user = await _repository.Update(user);

            return new Response<UserDTO>
            {
                Data = _mapper.Map<UserModel, UserDTO>(user)
            };
        }
    }
}
