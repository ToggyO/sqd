using Microsoft.AspNetCore.Mvc;
using Squadio.API.Handlers.Users;
using Squadio.Domain.Models.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Users;

namespace Squadio.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersHandler _handler;

        public UsersController(IUsersHandler handler)
        {
            _handler = handler;
        }
        
        [HttpGet]
        public async Task<UserModel> GetAll()
        {
            return null;
        }
        
        [HttpGet("{id}")]
        public async Task<Response<UserDTO>> GetById([Required, FromRoute] Guid id)
        {
            return await _handler.GetById(id);
        }
        
        [HttpPost("signup/{email}")]
        public async Task<Response> SignUp([Required, FromRoute] string email)
        {
            return await _handler.SignUp(email);
        }
        
        [HttpPut("passwords/set")]
        public async Task<Response<UserDTO>> SetPassword([FromBody] UserSetPasswordDTO dto)
        {
            return await _handler.SetPassword(dto);
        }
        
        [HttpPost("passwords/reset")]
        public async Task<Response> ResetPasswordRequest([FromQuery, Required] string email)
        {
            return await _handler.ResetPasswordRequest(email);
        }
    }
}