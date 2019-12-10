using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Squadio.API.Filters;
using Squadio.API.Handlers.Admins;
using Squadio.Common.Enums;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Companies;
using Squadio.DTO.Users;

namespace Squadio.API.Controllers
{
    [ApiController]
    [AuthorizationFilter]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminsHandler _handler;
        
        public AdminController(IAdminsHandler handler)
        {
            _handler = handler;
        }
        
        /// <summary>
        /// Get users with pagination
        /// </summary>
        [HttpGet("users")]
        [PermissionFilter(Area.Admin)]
        public async Task<Response<PageModel<UserWithCompaniesDTO>>> GetUsersPage([FromQuery] PageModel model
            , [FromQuery] UserWithCompaniesFilter filter
            , [FromQuery] string search)
        {
            return await _handler.GetUsersPage(model, search, filter);
        }
        
        /// <summary>
        /// Get companies with pagination
        /// </summary>
        [HttpGet("companies")]
        [PermissionFilter(Area.Admin)]
        public async Task<Response<PageModel<CompanyListDTO>>> GetCompaniesPage([FromQuery] PageModel model
            , [FromQuery] CompaniesFilter filter
            , [FromQuery] string search)
        {
            return await _handler.GetCompaniesPage(model, filter, search);
        }
        
        /// <summary>
        /// Get company details
        /// </summary>
        [HttpGet("companies/{id}")]
        [PermissionFilter(Area.Admin)]
        public async Task<Response<CompanyDetailDTO>> GetCompanyDetail([FromRoute] Guid id)
        {
            return await _handler.GetCompanyDetail(id);
        }
    }
}