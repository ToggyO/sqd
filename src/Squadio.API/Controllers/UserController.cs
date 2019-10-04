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
    public class UserController : ControllerBase
    {
        private readonly IUserHandler _userHandler;

        public UserController(IUserHandler userHandler)
        {
            _userHandler = userHandler;
        }
        
        [HttpGet]
        public async Task<UserModel> GetAll()
        {
            return null;
        }
        
        [HttpGet("{id}")]
        public async Task<Response<UserDTO>> GetById([Required, FromRoute] Guid id)
        {
            return await _userHandler.GetById(id);
        }
        
        [HttpGet("signup/{email}")]
        public async Task<Response> SignUpEmail([Required, FromRoute] string email)
        {
            return await _userHandler.SignUp(email);
        }
    }
}