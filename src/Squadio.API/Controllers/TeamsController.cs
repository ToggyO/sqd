using Microsoft.AspNetCore.Mvc;
using Squadio.API.Handlers.Teams;

namespace Squadio.API.Controllers
{
    [ApiController]
    [Route("api/teams")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamsHandler _handler;
        public TeamsController(ITeamsHandler handler)
        {
            _handler = handler;
        }
        
        [HttpGet]
        public string GetTeam()
        {
            return "Get from team controller";
        }
    }
}