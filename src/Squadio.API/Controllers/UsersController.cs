using Microsoft.AspNetCore.Mvc;
using Squadio.API.Handlers.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Pages;
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
        public async Task<Response<PageModel<UserDTO>>> GetById([FromQuery] PageModel model)
        {
            return await _handler.GetPage(model);
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
        
        [HttpPut("passwords/set")]
        public async Task<Response<UserDTO>> SetPassword([FromBody] UserSetPasswordDTO dto)
        {
            return await _handler.SetPassword(dto);
        }
        
        [HttpPost("passwords/request")]
        public async Task<Response> ResetPasswordRequest([FromBody, Required] UserEmailDTO dto)
        {
            return await _handler.ResetPasswordRequest(dto.Email);
        }
    }
}