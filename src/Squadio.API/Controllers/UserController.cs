using Microsoft.AspNetCore.Mvc;
using Squadio.API.Handlers.Users;

namespace Squadio.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserHandler _userHandler;

        public UserController(IUserHandler userHandler)
        {
            _userHandler = userHandler;
        }
    }
}