using Microsoft.AspNetCore.Mvc;
using Squadio.API.Handlers.Users;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Squadio.Common.Models.Responses;

namespace Squadio.API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/v1/versions")]
    public class VersionController : ControllerBase
    {
        private const string Version = "0.0.1 b";

        [HttpGet]
        public Response<string> GetVersion()
        {
            var result = new Response<string>
            {
                Data = Version
            };
            return result;
        }
    }
}