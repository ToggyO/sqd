using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Squadio.API.Filters;
using Squadio.API.Handlers.Admins;
using Squadio.API.Handlers.Auth;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Auth;
using Squadio.DTO.Companies;
using Squadio.DTO.Users;

namespace Squadio.API.Controllers
{
    [ApiController]
    //[AuthorizationFilter]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminsHandler _handler;
        
        public AdminController(IAdminsHandler handler)
        {
            _handler = handler;
        }
        
        [HttpGet("/users")]
        public async Task<Response<PageModel<UserWithCompaniesDTO>>> GetUsersPage([FromQuery] PageModel model
            , [FromQuery] UserWithCompaniesFilter filter
            , [FromQuery] string search)
        {
            return await _handler.GetUsersPage(model, search, filter);
        }
        
        [HttpGet("/companies")]
        public async Task<Response<PageModel<CompanyListDTO>>> GetCompaniesPage([FromQuery] PageModel model
            , [FromQuery] CompaniesFilter filter
            , [FromQuery] string search)
        {
            return await _handler.GetCompaniesPage(model, filter, search);
        }
    }
}