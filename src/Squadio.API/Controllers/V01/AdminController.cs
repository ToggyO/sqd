using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Squadio.API.Filters;
using Squadio.API.Handlers.Admins;
using Squadio.Common.Enums;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Models.Admin;
using Squadio.DTO.Models.Auth;
using Squadio.DTO.Models.Users;
using Squadio.DTO.Models.Users.Settings;

namespace Squadio.API.Controllers.V01
{
    [ApiController]
    [ApiVersion("0.1")]
    [Route("v{version:apiVersion}/admin")]
    [PermissionFilter(Area.Admin)]
    [ServiceFilter(typeof(AuthorizationFilter))]
    [ServiceFilter(typeof(UserStatusFilter))]
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
        /// Refresh access token using refresh token 
        /// </summary>
        [HttpPut("sign-in")]
        [AllowAnonymous]
        public async Task<Response> SignIn([FromBody] RefreshTokenDTO dto)
        {
            return await _handler.RefreshToken(dto.RefreshToken);
        }
        
        /// <summary>
        /// Get users with pagination
        /// </summary>
        [HttpGet("users")]
        public async Task<Response<PageModel<UserWithCompaniesDTO>>> GetUsersPage([FromQuery] PageModel model, [FromQuery] UserFilterAdminDTO filter)
        {
            return await _handler.GetUsersPage(model, filter);
        }
        
        /// <summary>
        /// Get details of specified user
        /// </summary>
        [HttpGet("users/{userId}/detail")]
        public async Task<Response<UserDetailAdminDTO>> GetUserDetail([FromRoute] Guid userId)
        {
            return await _handler.GetUserDetail(userId);
        }
        
        /// <summary>
        /// Block specified user
        /// </summary>
        [HttpPut("users/{userId}/block")]
        public async Task<Response> BlockUser([FromRoute] Guid userId)
        {
            return await _handler.BlockUser(userId);
        }
        
        /// <summary>
        /// Unblock specified user
        /// </summary>
        [HttpPut("users/{userId}/unblock")]
        public async Task<Response> UnblockUser([FromRoute] Guid userId)
        {
            return await _handler.UnblockUser(userId);
        }
        
        /// <summary>
        /// Get companies with pagination
        /// </summary>
        [HttpGet("companies")]
        public async Task<Response<PageModel<CompanyDetailAdminDTO>>> GetCompaniesPage([FromQuery] PageModel model, [FromQuery] CompanyFilterAdminDTO filter)
        {
            return await _handler.GetCompanyPage(model, filter);
        }
        
        /// <summary>
        /// Get details of specified company
        /// </summary>
        [HttpGet("companies/{companyId}/detail")]
        public async Task<Response<CompanyDetailAdminDTO>> GetCompaniesDetail([FromRoute] Guid companyId)
        {
            return await _handler.GetCompanyDetail(companyId);
        }
        
        /// <summary>
        /// Get users page of specified company
        /// </summary>
        [HttpGet("companies/{companyId}/users")]
        public async Task<Response<PageModel<UserWithRoleDTO>>> GetCompaniesUsersPage([FromQuery] PageModel model, [FromRoute] Guid companyId)
        {
            return await _handler.GetCompanyUsersPage(model, companyId);
        }
        
        /// <summary>
        /// Block specified company
        /// </summary>
        [HttpPut("companies/{companyId}/block")]
        public async Task<Response> BlockCompany([FromRoute] Guid companyId)
        {
            return await _handler.BlockCompany(companyId);
        }
        
        /// <summary>
        /// Unblock specified company
        /// </summary>
        [HttpPut("companies/{companyId}/unblock")]
        public async Task<Response> UnblockCompany([FromRoute] Guid companyId)
        {
            return await _handler.UnblockCompany(companyId);
        }
        
        /// <summary>
        /// Set new password 
        /// </summary>
        [HttpPut("password/change")]
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
        [HttpPut("password/reset/confirm")]
        [AllowAnonymous]
        public async Task<Response> ResetPassword([FromBody] UserResetPasswordDTO dto)
        {
            return await _handler.ResetPasswordConfirm(dto);
        }
        
        /// <summary>
        /// Send email for confirm new mailbox
        /// </summary>
        [HttpPost("email/set/request")]
        public async Task<Response> ChangeEmailRequest([FromBody, Required] UserChangeEmailRequestDTO dto)
        {
            return await _handler.ChangeEmailRequest(dto, User);
        }
        
        /// <summary>
        /// Set email from for current admin
        /// </summary>
        [HttpPut("email/set/confirm")]
        public async Task<Response<UserDTO>> SetEmail([FromBody] UserSetEmailDTO dto)
        {
            return await _handler.ChangeEmailConfirm(dto?.Code, User);
        }
    }
}