using Microsoft.AspNetCore.Mvc;
using Squadio.API.Handlers.Auth;

namespace Squadio.API.Controllers
{
    [ApiController]
    [Route("api/company")]
    public class CompanyController : ControllerBase
    {
        private readonly IAuthHandler _handler;

        public CompanyController(IAuthHandler handler)
        {
            _handler = handler;
        }
        
        [HttpGet]
        public string GetCompany()
        {
            return "Company";
        }
    }
}