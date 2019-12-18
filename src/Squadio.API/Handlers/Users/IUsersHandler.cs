using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Auth;
using Squadio.DTO.Companies;
using Squadio.DTO.Projects;
using Squadio.DTO.Resources;
using Squadio.DTO.Teams;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.Users
{
    public interface IUsersHandler
    {
        Task<Response<PageModel<UserDTO>>> GetPage(PageModel model);
        
        Task<Response<UserDTO>> GetById(Guid id);
        Task<Response<PageModel<CompanyUserDTO>>> GetUserCompanies(Guid id, PageModel model);
        Task<Response<PageModel<TeamUserDTO>>> GetUserTeams(Guid id, PageModel model);
        Task<Response<PageModel<ProjectUserDTO>>> GetUserProjects(Guid id, PageModel model);
        
        Task<Response<UserDTO>> GetCurrentUser(ClaimsPrincipal claims);
        Task<Response<PageModel<CompanyUserDTO>>> GetUserCompanies(ClaimsPrincipal claims, PageModel model);
        Task<Response<PageModel<TeamUserDTO>>> GetUserTeams(ClaimsPrincipal claims, PageModel model);
        Task<Response<PageModel<ProjectUserDTO>>> GetUserProjects(ClaimsPrincipal claims, PageModel model);
        
        Task<Response<UserDTO>> UpdateCurrentUser(UserUpdateDTO dto, ClaimsPrincipal claims);
        Task<Response<UserDTO>> ResetPassword(UserResetPasswordDTO dto);
        Task<Response> ValidateCode(string code);
        Task<Response> ChangePassword(UserChangePasswordDTO dto, ClaimsPrincipal claims);
        Task<Response> ResetPasswordRequest(string email);
        Task<Response<UserDTO>> DeleteUser(Guid id);
        Task<Response<SimpleTokenDTO>> ChangeEmailRequest(UserChangeEmailRequestDTO dto, ClaimsPrincipal claims);
        Task<Response> SendNewChangeEmailRequest(UserSendNewChangeEmailRequestDTO dto, ClaimsPrincipal claims);
        Task<Response<UserDTO>> SetEmail(UserSetEmailDTO dto, ClaimsPrincipal claims);
        Task<Response<UserDTO>> SaveNewAvatar(FileImageCreateDTO dto, ClaimsPrincipal claims);
        Task<Response<UserDTO>> SaveNewAvatar(ResourceImageCreateDTO dto, ClaimsPrincipal claims);
        Task<Response<UserDTO>> DeleteAvatar(ClaimsPrincipal claims);
    }
}
