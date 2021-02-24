using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Squadio.BLL.Providers.Users;
using Squadio.BLL.Services.Resources;
using Squadio.BLL.Services.Tokens;
using Squadio.BLL.Services.Users;
using Squadio.Common.Enums;
using Squadio.Common.Extensions;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Auth;
using Squadio.DTO.Resources;
using Squadio.DTO.Users;
using Squadio.DTO.Users.Settings;

namespace Squadio.API.Handlers.Users.Implementation
{
    public class UsersSettingsHandler : IUsersSettingsHandler
    {
        private readonly IUsersProvider _provider;
        private readonly IUsersService _service;
        private readonly ITokensService _tokensService;
        private readonly IResourcesService _resourcesService;
        private readonly IMapper _mapper;
        
        public UsersSettingsHandler(IUsersProvider provider
            , IUsersService service
            , ITokensService tokensService
            , IResourcesService resourcesService
            , IMapper mapper)
        {
            _service = service;
            _provider = provider;
            _tokensService = tokensService;
            _resourcesService = resourcesService;
            _mapper = mapper;
        }

        public async Task<Response<UserDTO>> UpdateCurrentUser(UserUpdateDTO dto, ClaimsPrincipal claims)
        {
            var result = await _service.UpdateUser(claims.GetUserId(), dto);
            return result;
        }

        public async Task<Response> ValidateCode(string code)
        {
            var result = await _provider.ValidateCode(code);
            return result;
        }

        public async Task<Response> SetPassword(UserSetPasswordDTO dto, ClaimsPrincipal claims)
        {
            var userResponse = await _provider.GetById(claims.GetUserId());
            if (!userResponse.IsSuccess)
                return userResponse;
            
            var oldPasswordCorrectResponse = await _tokensService.Authenticate(new CredentialsDTO
            {
                Email = userResponse.Data.Email,
                Password = dto.OldPassword
            });

            if (!oldPasswordCorrectResponse.IsSuccess)
                return oldPasswordCorrectResponse;

            return await _service.SetPassword(userResponse.Data.Email, dto.Password);
        }

        public async Task<Response> ResetPasswordRequest(string email)
        {
            await _service.ResetPasswordRequest(email);
            var result = new Response();
            return result;
        }
        public async Task<Response<UserDTO>> ResetPassword(UserResetPasswordDTO dto)
        {
            var result = await _service.ResetPassword(dto.Code, dto.Password);
            return result;
        }

        public async Task<Response<SimpleTokenDTO>> ChangeEmailRequest(UserChangeEmailRequestDTO dto, ClaimsPrincipal claims)
        {
            var userResponse = await _provider.GetById(claims.GetUserId());
            if (!userResponse.IsSuccess)
                return ErrorResponse.MapResponse<SimpleTokenDTO, UserDTO>(userResponse);
            
            var oldPasswordCorrectResponse = await _tokensService.Authenticate(new CredentialsDTO
            {
                Email = userResponse.Data.Email,
                Password = dto.Password
            });

            if (!oldPasswordCorrectResponse.IsSuccess)
                return ErrorResponse.MapResponse<SimpleTokenDTO, AuthInfoDTO>(oldPasswordCorrectResponse);
            
            var sendConfirmationResponse = await _service.ChangeEmailRequest(claims.GetUserId(), dto.NewEmail);
            if(!sendConfirmationResponse.IsSuccess)
                return ErrorResponse.MapResponse<SimpleTokenDTO>(sendConfirmationResponse);

            var tokenResponse = await _tokensService.CreateCustomToken(5
                , "ChangeEmail"
                , new Dictionary<string, string> {{"email", dto.NewEmail}});
            return tokenResponse;
        }

        public async Task<Response> SendNewChangeEmailRequest(UserSendNewChangeEmailRequestDTO dto, ClaimsPrincipal claims)
        {
            var validateResponse = await _tokensService.ValidateCustomToken(dto.Token, "ChangeEmail");
            if (!validateResponse.IsSuccess)
                return validateResponse;

            var result = await _service.ChangeEmailRequest(claims.GetUserId(), dto.NewEmail);
            return result;
        }

        public async Task<Response<UserDTO>> SetEmail(UserSetEmailDTO dto, ClaimsPrincipal claims)
        {
            var result = await _service.SetEmail(claims.GetUserId(), dto.Code);
            return result;
        }

        public async Task<Response<UserDTO>> SaveNewAvatar(FormImageCreateDTO dto, ClaimsPrincipal claims)
        {
            var imageCreateDTO = _mapper.Map<FormImageCreateDTO, ImageCreateDTO>(dto);
            var savingResourceResponse = await _resourcesService.CreateImageResource(claims.GetUserId(), FileGroup.Avatar, imageCreateDTO);
            if (!savingResourceResponse.IsSuccess)
            {
                return ErrorResponse.MapResponse<UserDTO, ResourceImageDTO>(savingResourceResponse);
            }
            return await _service.SaveNewAvatar(claims.GetUserId(), savingResourceResponse.Data.ResourceId);
        }

        public async Task<Response<UserDTO>> SaveNewAvatar(ByteImageCreateDTO dto, ClaimsPrincipal claims)
        {
            var imageCreateDTO = _mapper.Map<ByteImageCreateDTO, ImageCreateDTO>(dto);
            var savingResourceResponse = await _resourcesService.CreateImageResource(claims.GetUserId(), FileGroup.Avatar, imageCreateDTO);
            if (!savingResourceResponse.IsSuccess)
            {
                return ErrorResponse.MapResponse<UserDTO, ResourceImageDTO>(savingResourceResponse);
            }
            return await _service.SaveNewAvatar(claims.GetUserId(), savingResourceResponse.Data.ResourceId);
        }

        public async Task<Response<UserDTO>> DeleteAvatar(ClaimsPrincipal claims)
        {
            return await _service.DeleteAvatar(claims.GetUserId());
        }
    }
}