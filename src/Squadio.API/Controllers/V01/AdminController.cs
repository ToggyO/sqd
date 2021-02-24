using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Squadio.API.Filters;
using Squadio.API.Handlers.Admins;
using Squadio.BLL.Services.Notifications.Emails;
using Squadio.Common.Enums;
using Squadio.Common.Models.Emails;
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
        private readonly IEmailNotificationsService _email;
        
        public AdminController(IAdminsHandler handler
            , IEmailNotificationsService email)
        {
            _handler = handler;
            _email = email;
        }
        
        /// <summary>
        /// Create token using email and password 
        /// </summary>
        [HttpPost("test")]
        [AllowAnonymous]
        public async Task<Response> test()
        {
            var dto = new MailNotificationModel
            {
                ToAddresses = new []{"Karpov@magora-systems.com"},
                TemplateId = "TestTemplate",
                Html = true,
                Subject = "aaa",
                Body = "bbbbody"
            };
            return await _email.SendEmail(dto);
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
    }
}