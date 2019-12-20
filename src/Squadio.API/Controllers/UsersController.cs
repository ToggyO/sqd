using Microsoft.AspNetCore.Mvc;
using Squadio.API.Handlers.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Squadio.API.Filters;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Auth;
using Squadio.DTO.Companies;
using Squadio.DTO.Projects;
using Squadio.DTO.Resources;
using Squadio.DTO.Teams;
using Squadio.DTO.Users;
using Squadio.DTO.Users.Settings;

namespace Squadio.API.Controllers
{
    [ApiController]
    [AuthorizationFilter]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersHandler _handler;
        private readonly IUsersSettingsHandler _settingsHandler;

        public UsersController(IUsersHandler handler
            , IUsersSettingsHandler settingsHandler)
        {
            _handler = handler;
            _settingsHandler = settingsHandler;
        }
        
        /// <summary>
        /// Get users with pagination (for development purposes)
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
        public async Task<Response<PageModel<CompanyWithUserRoleDTO>>> GetUserCompanies([Required, FromRoute] Guid id
            , [FromQuery] PageModel model
            , [FromQuery] Guid? companyId)
        {
            return await _handler.GetUserCompanies(id, model, companyId);
        }
        
        /// <summary>
        /// Get teams of user
        /// </summary>
        [HttpGet("{id}/teams")]
        public async Task<Response<PageModel<TeamWithUserRoleDTO>>> GetUserTeams([Required, FromRoute] Guid id
            , [FromQuery] PageModel model
            , [FromQuery] Guid? companyId
            , [FromQuery] Guid? teamId)
        {
            return await _handler.GetUserTeams(id, model, companyId, teamId);
        }
        
        /// <summary>
        /// Get projects of user
        /// </summary>
        [HttpGet("{id}/projects")]
        public async Task<Response<PageModel<ProjectWithUserRoleDTO>>> GetUserProjects([Required, FromRoute] Guid id
            , [FromQuery] PageModel model
            , [FromQuery] Guid? companyId
            , [FromQuery] Guid? teamId
            , [FromQuery] Guid? projectId)
        {
            return await _handler.GetUserProjects(id, model, companyId, teamId, projectId);
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
        public async Task<Response<PageModel<CompanyWithUserRoleDTO>>> GetUserCompanies([FromQuery] PageModel model
            , [FromQuery] Guid? companyId)
        {
            return await _handler.GetUserCompanies(User, model, companyId);
        }
        
        /// <summary>
        /// Get teams current user
        /// </summary>
        [HttpGet("current/teams")]
        public async Task<Response<PageModel<TeamWithUserRoleDTO>>> GetUserTeams([FromQuery] PageModel model
            , [FromQuery] Guid? companyId
            , [FromQuery] Guid? teamId)
        {
            return await _handler.GetUserTeams(User, model, companyId, teamId);
        }
        
        /// <summary>
        /// Get projects current user
        /// </summary>
        [HttpGet("current/projects")]
        public async Task<Response<PageModel<ProjectWithUserRoleDTO>>> GetUserProjects([FromQuery] PageModel model
            , [FromQuery] Guid? companyId
            , [FromQuery] Guid? teamId
            , [FromQuery] Guid? projectId)
        {
            return await _handler.GetUserProjects(User, model, companyId, teamId, projectId);
        }
        
        /// <summary>
        /// Update current user
        /// </summary>
        [Obsolete]
        [HttpPut("current")]
        public async Task<Response<UserDTO>> UpdateCurrentUser([FromBody] UserUpdateDTO dto)
        {
            return await _settingsHandler.UpdateCurrentUser(dto, User);
        }
        
        /// <summary>
        /// Check the password restore code for validity
        /// </summary>
        [Obsolete]
        [HttpGet("passwords/validate")]
        [AllowAnonymous]
        public async Task<Response> ValidateCode([FromQuery, Required] string code)
        {
            return await _settingsHandler.ValidateCode(code);
        }
        
        /// <summary>
        /// Set new password 
        /// </summary>
        [Obsolete]
        [HttpPut("passwords/change")]
        public async Task<Response> ChangePassword([FromBody] UserSetPasswordDTO dto)
        {
            return await _settingsHandler.SetPassword(dto, User);
        }
        
        /// <summary>
        /// Set new password, using link from "api/passwords/request"
        /// </summary>
        [Obsolete]
        [HttpPut("passwords/set")]
        [AllowAnonymous]
        public async Task<Response<UserDTO>> SetPassword([FromBody] UserResetPasswordDTO dto)
        {
            return await _settingsHandler.ResetPassword(dto);
        }
        
        /// <summary>
        /// Send link to email for restore password
        /// </summary>
        [Obsolete]
        [HttpPost("passwords/request")]
        [AllowAnonymous]
        public async Task<Response> ResetPasswordRequest([FromBody, Required] UserEmailDTO dto)
        {
            return await _settingsHandler.ResetPasswordRequest(dto.Email);
        }
        
        /// <summary>
        /// Set email from "api/email/request" for current user
        /// </summary>
        [Obsolete]
        [HttpPut("email/set")]
        public async Task<Response<UserDTO>> SetEmail([FromBody] UserSetEmailDTO dto)
        {
            return await _settingsHandler.SetEmail(dto, User);
        }
        
        /// <summary>
        /// Send email for confirm new mailbox
        /// </summary>
        [Obsolete]
        [HttpPost("email/request")]
        public async Task<Response<SimpleTokenDTO>> ChangeEmailRequest([FromBody, Required] UserChangeEmailRequestDTO dto)
        {
            return await _settingsHandler.ChangeEmailRequest(dto, User);
        }
        
        /// <summary>
        /// Send new email for confirm new mailbox
        /// </summary>
        [Obsolete]
        [HttpPost("email/send-new-request")]
        public async Task<Response> SendNewChangeEmailRequest([FromBody, Required] UserSendNewChangeEmailRequestDTO dto)
        {
            return await _settingsHandler.SendNewChangeEmailRequest(dto, User);
        }
        
        /// <summary>
        /// Save new avatar for current user
        /// </summary>
        [Obsolete]
        [HttpPost("avatar-from-form")]
        public async Task<Response<UserDTO>> SaveNewAvatar([FromForm, Required] FileImageCreateDTO dto)
        {
            return await _settingsHandler.SaveNewAvatar(dto, User);
        }
        
        /// <summary>
        /// Save new avatar for current user
        /// </summary>
        [Obsolete]
        [HttpPost("avatar-from-body")]
        public async Task<Response<UserDTO>> SaveNewAvatar([FromBody, Required] ResourceImageCreateDTO dto)
        {
            return await _settingsHandler.SaveNewAvatar(dto, User);
        }
        
        /// <summary>
        /// Save new avatar for current user
        /// </summary>
        [Obsolete]
        [HttpDelete("avatar")]
        public async Task<Response<UserDTO>> DeleteAvatar()
        {
            return await _settingsHandler.DeleteAvatar(User);
        }
    }
}