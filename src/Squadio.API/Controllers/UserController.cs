using Microsoft.AspNetCore.Mvc;
using Squadio.API.Handlers.Users;
using Squadio.Domain.Models.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

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
        [HttpGet("{id}")]
        public async Task<UserModel> GetById([Required, FromRoute] Guid id)
        {
            return await _userHandler.GetById(id);
        }
    }
}