using System;
using System.Collections.Generic;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Magora.Passwords;
using Mapper;
using Microsoft.Extensions.Options;
using Squadio.BLL.Factories;
using Squadio.Common.Extensions;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;
using Squadio.Common.Settings;
using Squadio.DAL.Repository.SignUp;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Auth;
using Squadio.DTO.Users;  

namespace Squadio.BLL.Services.Tokens.Implementation
{
    public class TokensService : ITokensService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IPasswordService _passwordService;
        private readonly ISignUpRepository _signUpRepository;
        private readonly IMapper _mapper;
        private readonly ITokensFactory _tokenFactory;
        private readonly IOptions<GoogleSettings> _googleSettings;

        public TokensService(IUsersRepository usersRepository
            , IPasswordService passwordService
            , ISignUpRepository signUpRepository
            , IMapper mapper
            , ITokensFactory tokenFactory
            , IOptions<GoogleSettings> googleSettings)
        {
            _usersRepository = usersRepository;
            _passwordService = passwordService;
            _signUpRepository = signUpRepository;
            _mapper = mapper;
            _tokenFactory = tokenFactory;
            _googleSettings = googleSettings;
        }
        
        public async Task<Response<AuthInfoDTO>> Authenticate(CredentialsDTO dto)
        {
            var user = await _usersRepository.GetByEmail(dto.Email);
            var isPasswordValid = await ValidatePassword(dto.Password, user);
            
            if(user == null || !isPasswordValid)
            {
                return new SecurityErrorResponse<AuthInfoDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.AuthDataInvalid,
                        Message = ErrorMessages.Security.AuthDataInvalid
                    }
                });
            }

            var tokenDTO = await _tokenFactory.CreateToken(user);
            var userDTO = _mapper.Map<UserModel, UserDTO>(user);
            var step = await _signUpRepository.GetRegistrationStepByUserId(user.Id);
            var stepDTO = _mapper.Map<UserRegistrationStepModel, UserRegistrationStepDTO>(step);

            var result = new AuthInfoDTO
            {
                User = userDTO,
                RegistrationStep = stepDTO,
                Token = tokenDTO
            };

            return new Response<AuthInfoDTO>
            {
                Data = result
            };
        }

        public async Task<Response<TokenDTO>> RefreshToken(string refreshToken)
        {
            var isTokenValid = _tokenFactory.ValidateToken(refreshToken, out var tokenPrincipal);
            
            if (!isTokenValid)
            {
                return new ForbiddenErrorResponse<TokenDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.RefreshTokenInvalid,
                        Message = ErrorMessages.Security.RefreshTokenInvalid
                    }
                });
            }
            
            var userId = tokenPrincipal.GetUserId();
            if (!userId.HasValue)
            {
                return tokenPrincipal.Unauthorized<TokenDTO>();
            }

            var user = await _usersRepository.GetById(userId.Value);

            if (user == null)
            {
                return new BusinessConflictErrorResponse<TokenDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.UserDoesNotExists,
                        Message = ErrorMessages.Business.UserDoesNotExists,
                        Field = ErrorFields.User.Id
                    }
                });
            }

            var tokenDTO = await _tokenFactory.CreateToken(user);

            return new Response<TokenDTO>
            {
                Data = tokenDTO
            };
        }

        public async Task<Response<AuthInfoDTO>> GoogleAuthenticate(string googleToken)
        {
            GoogleJsonWebSignature.Payload infoFromGoogleToken;

            try
            {
                infoFromGoogleToken = await GoogleJsonWebSignature.ValidateAsync(googleToken);
            }
            catch
            {
                return new ForbiddenErrorResponse<AuthInfoDTO>(new[]
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.GoogleTokenInvalid,
                        Message = ErrorMessages.Security.GoogleTokenInvalid,
                        Field = ErrorFields.User.GoogleToken
                    }
                });
            }
            
            var user = await _usersRepository.GetByEmail(infoFromGoogleToken.Email);
            if(user == null)
            {
                return new BusinessConflictErrorResponse<AuthInfoDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.UserDoesNotExists,
                        Message = ErrorMessages.Business.UserDoesNotExists,
                        Field = ErrorFields.User.Email
                    }
                });
            }
            
            var tokenDTO = await _tokenFactory.CreateToken(user);
            var userDTO = _mapper.Map<UserModel, UserDTO>(user);
            var step = await _signUpRepository.GetRegistrationStepByUserId(user.Id);
            var stepDTO = _mapper.Map<UserRegistrationStepModel, UserRegistrationStepDTO>(step);

            var result = new AuthInfoDTO
            {
                User = userDTO,
                RegistrationStep = stepDTO,
                Token = tokenDTO
            };
            
            return new Response<AuthInfoDTO>
            {
                Data = result
            };
        }

        /// <summary>
        /// Return true if password is valid
        /// </summary>
        private async Task<bool> ValidatePassword(string password, UserModel user)
        {
            return user != null && await _passwordService.VerifyPassword(new PasswordModel
            {
                Hash = user.Hash,
                Salt = user.Salt
            }, password);
        }
    }
}