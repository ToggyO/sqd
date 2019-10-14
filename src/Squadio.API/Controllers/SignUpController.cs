using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Squadio.API.Handlers.SignUp;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Auth;
using Squadio.DTO.Users;

namespace Squadio.API.Controllers
{
    [ApiController]
    [Route("api/signup")]
    public class SignUpController : ControllerBase
    {
        private readonly ISignUpHandler _handler;

        public SignUpController(ISignUpHandler handler)
        {
            _handler = handler;
        }
        
        [HttpGet("step")]
        public async Task<Response<UserRegistrationStepDTO>> GetStatus([Required, FromQuery] UserEmailDTO dto)
        {
            return await _handler.GetRegistrationStep(dto.Email);
        }
        
        [HttpPost]
        public async Task<Response> SignUp([Required, FromBody] UserEmailDTO dto)
        {
            return await _handler.SignUp(dto.Email);
        }
        
        [HttpPut("password")]
        public async Task<Response<AuthInfoDTO>> SetPassword([FromBody] UserSetPasswordDTO dto)
        {
            return await _handler.SignUpPassword(dto);
        }
        
        [HttpPut("username")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Response<UserDTO>> UpdateCurrentUser([FromBody] UserUpdateDTO dto)
        {
            return await _handler.SignUpUsername(dto, User);
        }
    }
}