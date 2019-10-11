using Microsoft.AspNetCore.Mvc;
using Squadio.API.Handlers.Users;
using Squadio.Domain.Models.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Response<IEnumerable<UserDTO>>> GetAll()
        {
            return await _handler.GetAll();
        }
        
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Response<UserDTO>> GetById([Required, FromRoute] Guid id)
        {
            return await _handler.GetById(id);
        }
        
        [HttpGet("current")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Response<UserDTO>> GetCurrentUser()
        {
            return await _handler.GetCurrentUser(User);
        }
        
        [HttpPut("current")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Response<UserDTO>> UpdateCurrentUser([FromBody] UserUpdateDTO dto)
        {
            return await _handler.UpdateCurrentUser(dto, User);
        }
        
        [HttpPost("signup")]
        public async Task<Response> SignUp([Required, FromBody] UserEmailDTO email)
        {
            return await _handler.SignUp(email.Email);
        }
        
        [HttpPut("passwords/set")]
        public async Task<Response<UserDTO>> SetPassword([FromBody] UserSetPasswordDTO dto)
        {
            return await _handler.SetPassword(dto);
        }
        
        [HttpPost("passwords/reset")]
        public async Task<Response> ResetPasswordRequest([FromBody, Required] UserEmailDTO dto)
        {
            return await _handler.ResetPasswordRequest(dto.Email);
        }
    }
}