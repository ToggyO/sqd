using Microsoft.AspNetCore.Mvc;
using Squadio.API.Handlers.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Squadio.API.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Auth;
using Squadio.DTO.Companies;
using Squadio.DTO.Projects;
using Squadio.DTO.Resources;
using Squadio.DTO.Teams;
using Squadio.DTO.Users;

namespace Squadio.API.Controllers
{
    [ApiController]
    [AuthorizationFilter]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersHandler _handler;

        public UsersController(IUsersHandler handler)
        {
            _handler = handler;
        }
        
        /// <summary>
        /// Get users with pagination
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<Response<PageModel<UserDTO>>> GetPage([FromQuery] PageModel model)
        {
            return await _handler.GetPage(model);
        }
        
        /// <summary>
        /// Delete user by id (for development purposes)
        /// </summary>
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<Response<UserDTO>> DeleteUser([Required, FromRoute] Guid id)
        {
            return await _handler.DeleteUser(id);
        }
        
        /// <summary>
        /// Get user by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<Response<UserDTO>> GetById([Required, FromRoute] Guid id)
        {
            return await _handler.GetById(id);
        }
        
        /// <summary>
        /// Get companies of user
        /// </summary>
        [HttpGet("{id}/companies")]
        public async Task<Response<PageModel<CompanyUserDTO>>> GetUserCompanies([Required, FromRoute] Guid id, [FromQuery] PageModel model)
        {
            return await _handler.GetUserCompanies(id, model);
        }
        
        /// <summary>
        /// Get teams of user
        /// </summary>
        [HttpGet("{id}/teams")]
        public async Task<Response<PageModel<TeamUserDTO>>> GetUserTeams([Required, FromRoute] Guid id, [FromQuery] PageModel model)
        {
            return await _handler.GetUserTeams(id, model);
        }
        
        /// <summary>
        /// Get projects of user
        /// </summary>
        [HttpGet("{id}/projects")]
        public async Task<Response<PageModel<ProjectUserDTO>>> GetUserProjects([Required, FromRoute] Guid id, [FromQuery] PageModel model)
        {
            return await _handler.GetUserProjects(id, model);
        }
        
        /// <summary>
        /// Get current user
        /// </summary>
        [HttpGet("current")]
        public async Task<Response<UserDTO>> GetCurrentUser()
        {
            return await _handler.GetCurrentUser(User);
        }
        
        /// <summary>
        /// Get companies current user
        /// </summary>
        [HttpGet("current/companies")]
        public async Task<Response<PageModel<CompanyUserDTO>>> GetUserCompanies([FromQuery] PageModel model)
        {
            return await _handler.GetUserCompanies(User, model);
        }
        
        /// <summary>
        /// Get teams current user
        /// </summary>
        [HttpGet("current/teams")]
        public async Task<Response<PageModel<TeamUserDTO>>> GetUserTeams([FromQuery] PageModel model)
        {
            return await _handler.GetUserTeams(User, model);
        }
        
        /// <summary>
        /// Get projects current user
        /// </summary>
        [HttpGet("current/projects")]
        public async Task<Response<PageModel<ProjectUserDTO>>> GetUserProjects([FromQuery] PageModel model)
        {
            return await _handler.GetUserProjects(User, model);
        }
        
        /// <summary>
        /// Update current user
        /// </summary>
        [HttpPut("current")]
        public async Task<Response<UserDTO>> UpdateCurrentUser([FromBody] UserUpdateDTO dto)
        {
            return await _handler.UpdateCurrentUser(dto, User);
        }
        
        /// <summary>
        /// Check the password restore code for validity
        /// </summary>
        [HttpGet("passwords/validate")]
        [AllowAnonymous]
        public async Task<Response> ValidateCode([FromQuery, Required] string code)
        {
            return await _handler.ValidateCode(code);
        }
        
        /// <summary>
        /// Set new password 
        /// </summary>
        [HttpPut("passwords/change")]
        public async Task<Response> ChangePassword([FromBody] UserChangePasswordDTO dto)
        {
            return await _handler.ChangePassword(dto, User);
        }
        
        /// <summary>
        /// Set new password, using link from "api/passwords/request"
        /// </summary>
        [HttpPut("passwords/set")]
        [AllowAnonymous]
        public async Task<Response<UserDTO>> SetPassword([FromBody] UserResetPasswordDTO dto)
        {
            return await _handler.ResetPassword(dto);
        }
        
        /// <summary>
        /// Send link to email for restore password
        /// </summary>
        [HttpPost("passwords/request")]
        [AllowAnonymous]
        public async Task<Response> ResetPasswordRequest([FromBody, Required] UserEmailDTO dto)
        {
            return await _handler.ResetPasswordRequest(dto.Email);
        }
        
        /// <summary>
        /// Set email from "api/email/request" for current user
        /// </summary>
        [HttpPut("email/set")]
        public async Task<Response<UserDTO>> SetEmail([FromBody] UserSetEmailDTO dto)
        {
            return await _handler.SetEmail(dto, User);
        }
        
        /// <summary>
        /// Send email for confirm new mailbox
        /// </summary>
        [HttpPost("email/request")]
        public async Task<Response<SimpleTokenDTO>> ChangeEmailRequest([FromBody, Required] UserChangeEmailRequestDTO dto)
        {
            return await _handler.ChangeEmailRequest(dto, User);
        }
        
        /// <summary>
        /// Send new email for confirm new mailbox
        /// </summary>
        [HttpPost("email/send-new-request")]
        public async Task<Response> SendNewChangeEmailRequest([FromBody, Required] UserSendNewChangeEmailRequestDTO dto)
        {
            return await _handler.SendNewChangeEmailRequest(dto, User);
        }
        
        /// <summary>
        /// Save new avatar for current user
        /// </summary>
        [HttpPost("avatar-from-form")]
        public async Task<Response<UserDTO>> SaveNewAvatar([FromForm, Required] FileImageCreateDTO dto)
        {
            return await _handler.SaveNewAvatar(dto, User);
        }
        
        /// <summary>
        /// Save new avatar for current user
        /// </summary>
        [HttpPost("avatar-from-body")]
        public async Task<Response<UserDTO>> SaveNewAvatar([FromBody, Required] ResourceImageCreateDTO dto)
        {
            return await _handler.SaveNewAvatar(dto, User);
        }
        
        /// <summary>
        /// Save new avatar for current user
        /// </summary>
        [HttpDelete("avatar")]
        public async Task<Response<UserDTO>> DeleteAvatar()
        {
            return await _handler.DeleteAvatar(User);
        }
    }
}