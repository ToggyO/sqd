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
        private readonly IMapper _mapper;
        private readonly ITokensFactory _tokenFactory;
        private readonly IOptions<GoogleSettings> _googleSettings;

        public TokensService(IUsersRepository usersRepository
            , IPasswordService passwordService
            , IMapper mapper
            , ITokensFactory tokenFactory
            , IOptions<GoogleSettings> googleSettings)
        {
            _usersRepository = usersRepository;
            _passwordService = passwordService;
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
                return new ErrorResponse<AuthInfoDTO>
                {
                    Code = ErrorCodes.Security.AuthDataInvalid,
                    Message = ErrorMessages.Security.AuthDataInvalid,
                    HttpStatusCode = HttpStatusCode.Conflict
                };
            }

            var tokenDTO = await _tokenFactory.CreateToken(user);
            var userDTO = _mapper.Map<UserModel, UserDTO>(user);

            var result = new AuthInfoDTO
            {
                User = userDTO,
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
                return new ErrorResponse<TokenDTO>
                {
                    HttpStatusCode = HttpStatusCode.Forbidden,
                    Message = ErrorMessages.Security.Unauthorized,
                    Code = ErrorCodes.Security.Unauthorized,

                    Errors = new List<Error>
                    {
                        new Error
                        {
                            Message = ErrorMessages.Security.RefreshTokenInvalid,
                            Code = ErrorCodes.Security.RefreshTokenInvalid,
                            Field = "refreshToken"
                        }
                    }
                };
            }

            var user = await _usersRepository.GetById(tokenPrincipal.GetUserId());

            if (user == null)
            {
                return new ErrorResponse<TokenDTO>
                {
                    Code = ErrorCodes.Business.UserDoesNotExists,
                    Message = ErrorMessages.Business.UserDoesNotExists,
                    HttpStatusCode = HttpStatusCode.BadRequest,
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

            var tokenDTO = await _tokenFactory.CreateToken(user);

            return new Response<TokenDTO>
            {
                Data = tokenDTO
            };
        }

        public async Task<Response<AuthInfoDTO>> GoogleAuthenticate(string googleToken)
        {
            var infoFromGoogleToken = await GoogleJsonWebSignature.ValidateAsync(googleToken);

            if ((string) infoFromGoogleToken.Audience != _googleSettings.Value.ClientId)
            {
                return new ErrorResponse<AuthInfoDTO>
                {
                    Code = ErrorCodes.Security.GoogleAccessTokenInvalid,
                    Message = ErrorMessages.Security.GoogleAccessTokenInvalid,
                    HttpStatusCode = HttpStatusCode.BadRequest
                };
            }
            
            var user = await _usersRepository.GetByEmail(infoFromGoogleToken.Email);
            if(user == null)
            {
                return new ErrorResponse<AuthInfoDTO>
                {
                    Code = ErrorCodes.Business.UserDoesNotExists,
                    Message = ErrorMessages.Business.UserDoesNotExists,
                    HttpStatusCode = HttpStatusCode.BadRequest,
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
            
            var tokenDTO = await _tokenFactory.CreateToken(user);
            var userDTO = _mapper.Map<UserModel, UserDTO>(user);

            var result = new AuthInfoDTO
            {
                User = userDTO,
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