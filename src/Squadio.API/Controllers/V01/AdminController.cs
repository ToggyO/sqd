﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Squadio.API.Filters;
using Squadio.API.Handlers.Admins;
using Squadio.Common.Enums;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Models.Auth;
using Squadio.DTO.Models.Users;
using Squadio.DTO.Models.Users.Settings;

namespace Squadio.API.Controllers.V01
{
    [ApiController]
    [AuthorizationFilter]
    [ApiVersion("0.1")]
    [Route("api/v{version:apiVersion}/admin")]
    [PermissionFilter(Area.Admin)]
    public class AdminController : ControllerBase
    {
        private readonly IAdminsHandler _handler;
        
        public AdminController(IAdminsHandler handler)
        {
            _handler = handler;
        }
        
        /// <summary>
        /// Create token using email and password 
        /// </summary>
        [HttpPost("sign-in")]
        [AllowAnonymous]
        public async Task<Response> SignIn([FromBody] CredentialsDTO dto)
        {
            return await _handler.Authenticate(dto);
        }
        
        /// <summary>
        /// Get users with pagination
        /// </summary>
        [HttpGet("users")]
        public async Task<Response<PageModel<UserDTO>>> GetUsersPage([FromQuery] PageModel model)
        {
            return await _handler.GetUsersPage(model);
        }
        
        /// <summary>
        /// Set new password 
        /// </summary>
        [HttpPut("passwords/change")]
        public async Task<Response> ChangePassword([FromBody] UserSetPasswordDTO dto)
        {
            return await _handler.ChangePassword(dto, User);
        }
        
        /// <summary>
        /// Send link to email for restore password
        /// </summary>
        [HttpPost("password/reset/request")]
        [AllowAnonymous]
        public async Task<Response> ResetPasswordRequest([FromBody] UserEmailDTO dto)
        {
            return await _handler.ResetPasswordRequest(dto.Email);
        }
        
        /// <summary>
        /// Set new password, using code from email
        /// </summary>
        [HttpPut("password/reset")]
        [AllowAnonymous]
        public async Task<Response> ResetPassword([FromBody] UserResetPasswordDTO dto)
        {
            return await _handler.ResetPassword(dto);
        }
    }
}