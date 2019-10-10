using System;
using System.Net.Http.Headers;
using System.Security;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Magora.Passwords;
using Mapper;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Squadio.BLL.Factories;
using Squadio.Common.Extensions;
using Squadio.Common.Settings;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Auth;
using Squadio.DTO.Users;  
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using ClientCredential = Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential;
using UserAssertion = Microsoft.IdentityModel.Clients.ActiveDirectory.UserAssertion;

namespace Squadio.BLL.Services.Tokens.Implementation
{
    public class TokensService : ITokensService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IPasswordService _passwordService;
        private readonly IMapper _mapper;
        private readonly ITokensFactory _tokenFactory;
        private readonly IOptions<GoogleSettings> _googleSettings;
        private readonly IOptions<MicrosoftAccountSettings> _microsoftSettings;

        public TokensService(IUsersRepository usersRepository
            , IPasswordService passwordService
            , IMapper mapper
            , ITokensFactory tokenFactory
            , IOptions<GoogleSettings> googleSettings
            , IOptions<MicrosoftAccountSettings> microsoftSettings)
        {
            _usersRepository = usersRepository;
            _passwordService = passwordService;
            _mapper = mapper;
            _tokenFactory = tokenFactory;
            _googleSettings = googleSettings;
            _microsoftSettings = microsoftSettings;
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

        public async Task<AuthInfoDTO> GoogleAuthenticate(string gmailToken)
        {
            var infoFromGoogleToken = await GoogleJsonWebSignature.ValidateAsync(gmailToken);
            
            if((string) infoFromGoogleToken.Audience != _googleSettings.Value.ClientId)
                throw new SecurityException("Incorrect google token");
            
            var user = await _usersRepository.GetByEmail(infoFromGoogleToken.Email);
            if(user == null)
                throw new SecurityException("User not exist incorrect");
            
            var tokenDTO = await _tokenFactory.CreateToken(user);
            var userDTO = _mapper.Map<UserModel, UserDTO>(user);

            var result = new AuthInfoDTO
            {
                User = userDTO,
                Token = tokenDTO
            };
            
            return result;
        }

        public async Task<AuthInfoDTO> MicrosoftAuthenticate(string microsoftToken)
        {
            try
            {
                var userAssertion = new UserAssertion(microsoftToken);

                var authContext = new AuthenticationContext("https://login.microsoftonline.com/"+_microsoftSettings.Value.TenantId);
                var clientCredential = new ClientCredential(_microsoftSettings.Value.ClientId, _microsoftSettings.Value.ClientSecret);

                var result = await authContext.AcquireTokenAsync("https://graph.microsoft.com", clientCredential, userAssertion);

                int a = 0;
                /*
                var microsoftClient = PublicClientApplicationBuilder.Create(_microsoftSettings.Value.ClientId).Build();
                var aaa = await microsoftClient.AcquireTokenInteractive(new[] {"user.read"}).ExecuteAsync();
                */
            }
            catch (Exception ex)
            {
                throw;
            }
            return null;
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