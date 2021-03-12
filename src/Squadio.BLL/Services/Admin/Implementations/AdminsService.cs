using System;
using System.Threading.Tasks;
using AutoMapper;
using Magora.Passwords;
using Squadio.BLL.Services.Notifications.Emails;
using Squadio.Common.Helpers;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.ChangeEmail;
using Squadio.DAL.Repository.ChangePassword;
using Squadio.DAL.Repository.Companies;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Models.Users;

namespace Squadio.BLL.Services.Admin.Implementations
{
    public class AdminsService : IAdminsService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IChangePasswordRequestRepository _changePasswordRepository;
        private readonly IPasswordService _passwordService;
        private readonly IEmailNotificationsService _emailNotificationsService;
        private readonly ICompaniesRepository _companiesRepository;
        private readonly IChangeEmailRequestRepository _changeEmailRepository;
        private readonly IMapper _mapper;

        public AdminsService(IUsersRepository usersRepository
            , IChangePasswordRequestRepository changePasswordRepository
            , IPasswordService passwordService
            , IEmailNotificationsService emailNotificationsService
            , ICompaniesRepository companiesRepository
            , IChangeEmailRequestRepository changeEmailRepository
            , IMapper mapper
        )
        {
            _usersRepository = usersRepository;
            _changePasswordRepository = changePasswordRepository;
            _passwordService = passwordService;
            _emailNotificationsService = emailNotificationsService;
            _companiesRepository = companiesRepository;
            _changeEmailRepository = changeEmailRepository;
            _mapper = mapper;
        }

        public async Task<Response> SetPassword(string email, string password)
        {
            var user = await _usersRepository.GetByEmail(email);

            var passwordModel = await _passwordService.CreatePassword(password);
            await _usersRepository.SavePassword(user.Id, passwordModel.Hash, passwordModel.Salt);

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
            await _usersRepository.SavePassword(userPasswordRequest.UserId, passwordModel.Hash, passwordModel.Salt);

            await _changePasswordRepository.ActivateAllRequestsForUser(userPasswordRequest.UserId);
            return new Response();
        }

        public async Task<Response> ResetPasswordRequest(string email)
        {
            var user = await _usersRepository.GetByEmail(email);
            if (user == null)
            {
                return new Response();
            }

            if (user.RoleId != RoleGuid.Admin)
            {
                return new Response();
            }

            var code = CodeHelper.GenerateCodeAsGuid(3);

            await _changePasswordRepository.ActivateAllRequestsForUser(user.Id);

            await _changePasswordRepository.AddRequest(user.Id, code);

            return await _emailNotificationsService.SendResetAdminPasswordEmail(user.Email, code);
        }

        public async Task<Response> ChangeEmailRequest(Guid id, string newEmail)
        {
            var user = await _usersRepository.GetById(id);

            var checkEmail = await _usersRepository.GetByEmail(newEmail);
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

            var code = CodeHelper.GenerateCodeAsGuid();

            await _changeEmailRepository.AddRequest(user.Id, code, newEmail);

            return await _emailNotificationsService.SendConfirmNewAdminMailboxEmail(newEmail, code);
        }

        public async Task<Response<UserDTO>> ChangeEmailConfirm(Guid id, string code)
        {
            var user = await _usersRepository.GetById(id);
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

            var checkEmail = await _usersRepository.GetByEmail(request.NewEmail);
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

            user = await _usersRepository.Update(user);

            return new Response<UserDTO>
            {
                Data = _mapper.Map<UserModel, UserDTO>(user)
            };
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

            await _usersRepository.Create(entity);
            
            return new Response();
        }

        public async Task<Response> SetUserStatus(Guid userId, UserStatus status)
        {
            var userEntity = await _usersRepository.GetById(userId);
            
            if (userEntity == null)
            {
                return new NotFoundErrorResponse();
            }
            
            if (userEntity.RoleId == RoleGuid.Admin)
            {
                return new ForbiddenErrorResponse();
            }
            
            if(userEntity.Status == status)
                return new Response();

            userEntity.Status = status;
            await _usersRepository.Update(userEntity);
            return new Response();
        }

        public async Task<Response> SetCompanyStatus(Guid companyId, CompanyStatus status)
        {
            var companyEntity = await _companiesRepository.GetById(companyId);
            
            if (companyEntity == null)
            {
                return new NotFoundErrorResponse();
            }
            
            if(companyEntity.Status == status)
                return new Response();

            companyEntity.Status = status;
            await _companiesRepository.Update(companyEntity);
            return new Response();
        }
    }
}