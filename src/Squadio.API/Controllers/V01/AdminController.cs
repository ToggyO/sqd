using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Squadio.API.Filters;
using Squadio.API.Handlers.Admins;
using Squadio.Common.Enums;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Auth;
using Squadio.DTO.Users;
using Squadio.DTO.Users.Settings;

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
        /// Get users with pagination
        /// </summary>
        [HttpGet("users")]
        public async Task<Response<PageModel<UserDTO>>> GetUsersPage([FromQuery] PageModel model)
        {
            return await _handler.GetUsersPage(model);
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
        /// Set new password 
        /// </summary>
        [HttpPut("passwords/change")]
        public async Task<Response> ChangePassword([FromBody] UserSetPasswordDTO dto)
        {
            return await _handler.ChangePassword(dto, User);
        }
    }
}