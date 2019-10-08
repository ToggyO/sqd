﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using Magora.Passwords;
using Mapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Squadio.BLL.Factories;
using Squadio.BLL.Providers.Users;
using Squadio.Common.Extensions;
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

        public TokensService(IUsersRepository usersRepository
            , IPasswordService passwordService
            , IMapper mapper
            , ITokensFactory tokenFactory)
        {
            _usersRepository = usersRepository;
            _passwordService = passwordService;
            _mapper = mapper;
            _tokenFactory = tokenFactory;
        }
        
        public async Task<AuthInfoDTO> Authenticate(CredentialsDTO dto)
        {
            var user = await _usersRepository.GetByEmail(dto.Email);
            var isPasswordValid = await ValidatePassword(dto.Password, user);
            
            if(user == null || !isPasswordValid)
                throw new SecurityException("Email or password incorrect");

            var tokenDTO = await _tokenFactory.CreateToken(user);
            var userDTO = _mapper.Map<UserModel, UserDTO>(user);

            var result = new AuthInfoDTO
            {
                User = userDTO,
                Token = tokenDTO
            };
            
            return result;
        }

        public async Task<TokenDTO> RefreshToken(string refreshToken)
        {
            var isTokenValid = _tokenFactory.ValidateToken(refreshToken, out var tokenPrincipal);
            
            if (!isTokenValid)
                throw new SecurityException("Refresh token invalid");

            var user = await _usersRepository.GetById(tokenPrincipal.GetUserId() 
                                                    ?? throw new SecurityException("Refresh token invalid")) 
                       ?? throw new SecurityException("Refresh token invalid");
            
            var tokenDTO = await _tokenFactory.CreateToken(user);
            return tokenDTO;
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