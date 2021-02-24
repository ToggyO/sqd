﻿using System;
using System.Threading.Tasks;
using AutoMapper;
using Magora.Passwords;
using Squadio.BLL.Services.Resources;
using Squadio.Common.Helpers;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.ChangeEmail;
using Squadio.DAL.Repository.ChangePassword;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Models.Users;
using Squadio.DTO.Models.Users.Settings;

namespace Squadio.BLL.Services.Users.Implementations
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _repository;
        private readonly IChangePasswordRequestRepository _changePasswordRepository;
        private readonly IChangeEmailRequestRepository _changeEmailRepository;
        private readonly IPasswordService _passwordService;
        private readonly IMapper _mapper;
        private readonly IResourcesService _resourcesService;
        public UsersService(IUsersRepository repository
            , IChangePasswordRequestRepository changePasswordRepository
            , IChangeEmailRequestRepository changeEmailRepository
            , IPasswordService passwordService
            , IMapper mapper
            , IResourcesService resourcesService)
        {
            _repository = repository;
            _changePasswordRepository = changePasswordRepository;
            _changeEmailRepository = changeEmailRepository;
            _passwordService = passwordService;
            _mapper = mapper;
            _resourcesService = resourcesService;
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
            
            return new Response();
        }

        public async Task<Response<UserDTO>> CreateUser(UserCreateDTO dto)
        {
            var entity = new UserModel
            {
                Name = dto.Name,
                RoleId = RoleGuid.User,
                Email = dto.Email,
                CreatedDate = DateTime.UtcNow,
                Status = dto.UserStatus
            };
            
            entity = await _repository.Create(entity);
            
            var result = _mapper.Map<UserModel, UserDTO>(entity);
            
            return new Response<UserDTO>
            {
                Data = result
            };
        }

        public async Task<Response<UserDTO>> CreateUserWithPasswordRestore(UserCreateDTO dto, string code)
        {
            var user = await CreateUser(dto);
            await _changePasswordRepository.AddRequest(user.Data.Id, code);
            return user;
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
            await DeleteAvatar(id);
            
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

            var code = CodeHelper.GenerateNumberCode();

            await _changeEmailRepository.AddRequest(user.Id, code, newEmail);

            return new Response();
        }

        public async Task<Response<UserDTO>> SetEmail(Guid id, string code)
        {
            var user = await _repository.GetById(id);
            var request = await _changeEmailRepository.GetRequest(user.Id, code);

            if (request == null || request?.IsDeleted == true)
            {
                return new ForbiddenErrorResponse<UserDTO>(new []
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

        public async Task<Response<UserDTO>> SaveNewAvatar(Guid userId, Guid resourceId)
        {
            var userEntity = await _repository.GetById(userId);
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

            if (userEntity.AvatarId != null)
            {
                var deleteResult = await _resourcesService.DeleteResource(userEntity.AvatarId.Value);
            }

            userEntity.AvatarId = resourceId;
            userEntity = await _repository.Update(userEntity);
            
            var result = _mapper.Map<UserModel, UserDTO>(userEntity);
            return new Response<UserDTO>
            {
                Data = result
            };
        }

        public async Task<Response<UserDTO>> DeleteAvatar(Guid userId)
        {
            var userEntity = await _repository.GetById(userId);
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

            var avatar = userEntity.Avatar;
            if (avatar != null)
            {
                var deleteResult = await _resourcesService.DeleteResource(avatar.Id);
            }
            
            var result = _mapper.Map<UserModel, UserDTO>(userEntity);
            return new Response<UserDTO>
            {
                Data = result
            };
        }
    }
}