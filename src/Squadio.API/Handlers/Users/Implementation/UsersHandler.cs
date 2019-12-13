using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.BLL.Providers.Companies;
using Squadio.BLL.Providers.Projects;
using Squadio.BLL.Providers.Teams;
using Squadio.BLL.Providers.Users;
using Squadio.BLL.Services.Resources;
using Squadio.BLL.Services.Tokens;
using Squadio.BLL.Services.Users;
using Squadio.Common.Enums;
using Squadio.Common.Extensions;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Auth;
using Squadio.DTO.Companies;
using Squadio.DTO.Projects;
using Squadio.DTO.Resources;
using Squadio.DTO.Teams;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.Users.Implementation
{
    public class UsersHandler : IUsersHandler
    {
        private readonly IUsersProvider _provider;
        private readonly ICompaniesProvider _companyProvider;
        private readonly ITeamsProvider _teamsProvider;
        private readonly IProjectsProvider _projectsProvider;
        private readonly IUsersService _service;
        private readonly ITokensService _tokensService;
        private readonly IResourcesService _resourcesService;
        public UsersHandler(IUsersProvider provider
            , ICompaniesProvider companyProvider
            , ITeamsProvider teamsProvider
            , IProjectsProvider projectsProvider
            , IUsersService service
            , ITokensService tokensService
            , IResourcesService resourcesService)
        {
            _service = service;
            _companyProvider = companyProvider;
            _teamsProvider = teamsProvider;
            _projectsProvider = projectsProvider;
            _provider = provider;
            _tokensService = tokensService;
            _resourcesService = resourcesService;
        }

        public async Task<Response<PageModel<UserDTO>>> GetPage(PageModel model)
        {
            var result = await _provider.GetPage(model);
            return result;
        }

        public async Task<Response<UserDTO>> GetCurrentUser(ClaimsPrincipal claims)
        {
            var result = await _provider.GetById(claims.GetUserId());
            return result;
        }

        public async Task<Response<PageModel<CompanyUserDTO>>> GetUserCompanies(ClaimsPrincipal claims, PageModel model)
        {
            var result = await _companyProvider.GetUserCompanies(claims.GetUserId(), model);
            return result;
        }

        public async Task<Response<PageModel<TeamUserDTO>>> GetUserTeams(ClaimsPrincipal claims, PageModel model)
        {
            var result = await _teamsProvider.GetUserTeams(claims.GetUserId(), model);
            return result;
        }

        public async Task<Response<PageModel<ProjectUserDTO>>> GetUserProjects(ClaimsPrincipal claims, PageModel model)
        {
            var result = await _projectsProvider.GetUserProjects(claims.GetUserId(), model);
            return result;
        }

        public async Task<Response<UserDTO>> UpdateCurrentUser(UserUpdateDTO dto, ClaimsPrincipal claims)
        {
            var result = await _service.UpdateUser(claims.GetUserId(), dto);
            return result;
        }

        public async Task<Response<UserDTO>> GetById(Guid id)
        {
            var result = await _provider.GetById(id);
            return result;
        }

        public async Task<Response<PageModel<CompanyUserDTO>>> GetUserCompanies(Guid id, PageModel model)
        {
            var result = await _companyProvider.GetUserCompanies(id, model);
            return result;
        }

        public async Task<Response<PageModel<TeamUserDTO>>> GetUserTeams(Guid id, PageModel model)
        {
            var result = await _teamsProvider.GetUserTeams(id, model);
            return result;
        }

        public async Task<Response<PageModel<ProjectUserDTO>>> GetUserProjects(Guid id, PageModel model)
        {
            var result = await _projectsProvider.GetUserProjects(id, model);
            return result;
        }

        public async Task<Response> ValidateCode(string code)
        {
            var result = await _provider.ValidateCode(code);
            return result;
        }

        public async Task<Response> ChangePassword(UserChangePasswordDTO dto, ClaimsPrincipal claims)
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

        public async Task<Response<UserDTO>> DeleteUser(Guid id)
        {
            var result = await _service.DeleteUser(id);
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

        public async Task<Response<UserDTO>> SaveNewAvatar(FileImageCreateDTO dto, ClaimsPrincipal claims)
        {
            var savingResourceResponse = await _resourcesService.CreateResource(claims.GetUserId(), FileGroup.Avatar, dto);
            if (!savingResourceResponse.IsSuccess)
            {
                return ErrorResponse.MapResponse<UserDTO, ResourceImageDTO>(savingResourceResponse);
            }
            return await _service.SaveNewAvatar(claims.GetUserId(), savingResourceResponse.Data.ResourceId);
        }

        public async Task<Response<UserDTO>> SaveNewAvatar(ResourceImageCreateDTO dto, ClaimsPrincipal claims)
        {
            var savingResourceResponse = await _resourcesService.CreateResource(claims.GetUserId(), FileGroup.Avatar, dto);
            if (!savingResourceResponse.IsSuccess)
            {
                return ErrorResponse.MapResponse<UserDTO, ResourceImageDTO>(savingResourceResponse);
            }
            return await _service.SaveNewAvatar(claims.GetUserId(), savingResourceResponse.Data.ResourceId);
        }
    }
}
