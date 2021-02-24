using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Squadio.API.Filters;
using Squadio.API.Handlers.Users;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Enums;
using Squadio.DTO.Models.Auth;
using Squadio.DTO.Models.Resources;
using Squadio.DTO.Models.Users;
using Squadio.DTO.Models.Users.Settings;

namespace Squadio.API.Controllers.V01
{
    [ApiController]
    [AuthorizationFilter]
    [ApiVersion("0.1")]
    [Route("api/v{version:apiVersion}/users/settings")]
    public class UsersSettingsController : ControllerBase
    {
        private readonly IUsersSettingsHandler _handler;

        public UsersSettingsController(IUsersSettingsHandler handler)
        {
            _handler = handler;
        }
        
        /// <summary>
        /// Update current user
        /// </summary>
        [HttpPut]
        public async Task<Response<UserDTO>> UpdateCurrentUser([FromBody] UserUpdateDTO dto)
        {
            return await _handler.UpdateCurrentUser(dto, User);
        }
        
        /// <summary>
        /// Check the password restore code for validity
        /// </summary>
        [HttpGet("password/validate")]
        [AllowAnonymous]
        public async Task<Response> ValidateCode([FromQuery, Required] string code)
        {
            return await _handler.ValidateCode(code);
        }
        
        /// <summary>
        /// Set new password 
        /// </summary>
        [HttpPut("password/set")]
        public async Task<Response> SetPassword([FromBody] UserSetPasswordDTO dto)
        {
            return await _handler.SetPassword(dto, User);
        }
        
        /// <summary>
        /// Set new password, using code from email
        /// </summary>
        [HttpPut("password/reset")]
        [AllowAnonymous]
        public async Task<Response<UserDTO>> ResetPassword([FromBody] UserResetPasswordDTO dto)
        {
            return await _handler.ResetPassword(dto);
        }
        
        /// <summary>
        /// Send link to email for restore password
        /// </summary>
        [HttpPost("password/request")]
        [AllowAnonymous]
        public async Task<Response> ResetPasswordRequest([FromBody, Required] UserEmailDTO dto)
        {
            return await _handler.ResetPasswordRequest(dto.Email);
        }
        
        /// <summary>
        /// Set email from for current user
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
        public async Task<Response<UserDTO>> SaveNewAvatar([FromForm, Required] FormImageCreateDTO dto)
        {
            return await _handler.SaveNewAvatar(dto, User);
        }
        
        /// <summary>
        /// Save new avatar for current user
        /// </summary>
        [HttpPost("avatar-from-body")]
        public async Task<Response<UserDTO>> SaveNewAvatar([FromBody, Required] ByteImageCreateDTO dto)
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