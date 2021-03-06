using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Google.Apis.Auth;
using Magora.Passwords;
using Microsoft.Extensions.Logging;
using Squadio.BLL.Factories;
using Squadio.Common.Enums;
using Squadio.Common.Extensions;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Models.Auth;
using Squadio.DTO.Models.Users;

namespace Squadio.BLL.Services.Tokens.Implementations
{
    public class TokensService : ITokensService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IPasswordService _passwordService;
        private readonly IMapper _mapper;
        private readonly ITokensFactory _tokenFactory;

        private readonly ILogger<TokensService> _logger;

        public TokensService(IUsersRepository usersRepository
            , IPasswordService passwordService
            , IMapper mapper
            , ITokensFactory tokenFactory
            , ILogger<TokensService> logger
            //, IOptions<GoogleSettings> googleSettings
            )
        {
            _usersRepository = usersRepository;
            _passwordService = passwordService;
            _mapper = mapper;
            _tokenFactory = tokenFactory;
            _logger = logger;
            //_googleSettings = googleSettings;
        }

        public async Task<Response<SimpleTokenDTO>> CreateCustomToken(int lifeTime, string tokenName, Dictionary<string, string> claims = null)
        {
            var token = await _tokenFactory.CreateCustomToken(lifeTime, tokenName, claims);
            return new Response<SimpleTokenDTO>
            {
                Data = token
            };
        }

        public async Task<Response> ValidateCustomToken(string token, string tokenName)
        {
            var valid = _tokenFactory.ValidateCustomToken(token, tokenName);
            if(valid == TokenStatus.Valid)
                return new Response();
            
            return new ForbiddenErrorResponse(new Error
            {
                Code = ErrorCodes.Security.TokenInvalid,
                Message = ErrorMessages.Security.TokenInvalid
            });
        }

        public async Task<Response<AuthInfoDTO>> Authenticate(CredentialsDTO dto)
        {
            var user = await _usersRepository.GetByEmail(dto.Email);

            var isPasswordValid = false;

            try
            {
                isPasswordValid = await ValidatePassword(dto.Password, user);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return new SecurityErrorResponse<AuthInfoDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.AuthDataInvalid,
                        Message = ErrorMessages.Security.AuthDataInvalid
                    }
                });
            }
            
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
            var tokenStatus = _tokenFactory.ValidateToken(refreshToken, out var tokenPrincipal);
            
            if (tokenStatus != TokenStatus.Valid)
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
            if (userId == Guid.Empty)
            {
                return new SecurityErrorResponse<TokenDTO>(new Error
                {
                    Code = ErrorCodes.Security.Unauthorized,
                    Field = ErrorFields.User.Token,
                    Message = ErrorMessages.Security.Unauthorized
                });
            }

            var user = await _usersRepository.GetById(userId);

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
            
            //TODO: think how validate google token
            /*
            if ((string) infoFromGoogleToken.Audience != _googleSettings.Value.ClientId)
            {
                return new PermissionDeniedErrorResponse<AuthInfoDTO>(new[]
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.GoogleTokenInvalid,
                        Message = ErrorMessages.Security.GoogleTokenInvalid,
                        Field = ErrorFields.User.GoogleToken
                    }
                });
            }
            */
            
            var user = await _usersRepository.GetByEmail(infoFromGoogleToken.Email);
            if(user == null)
            {
                return new BusinessConflictErrorResponse<AuthInfoDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.UserDoesNotExists,
                        Message = ErrorMessages.Business.UserDoesNotExists,
                        Field = ErrorFields.User.GoogleToken
                    }
                });
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