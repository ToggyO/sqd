using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Squadio.API.Filters;
using Squadio.API.Handlers.Admins;
using Squadio.Common.Enums;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.Common.Models.Sorts;
using Squadio.DTO.Companies;
using Squadio.DTO.Users;
using Squadio.DTO.Users.Settings;

namespace Squadio.API.Controllers.V01
{
    [ApiController]
    [AuthorizationFilter]
    [ApiVersion("0.1")]
    [Route("api/v{version:apiVersion}/admin")]
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
            , [FromQuery] CompanyAdminFilter filter
            , [FromQuery] SortCompaniesModel sort
            , [FromQuery] string search)
        {
            return await _handler.GetCompaniesPage(model, filter, sort, search);
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
        
        /// <summary>
        /// Set new password 
        /// </summary>
        [HttpPut("passwords/change")]
        public async Task<Response> ChangePassword([FromBody] UserSetPasswordDTO dto)
        {
            return await _handler.ChangePassword(dto, User);
        }
        
        /// <summary>
        /// Set new password, using link from "api/passwords/request"
        /// </summary>
        [HttpPut("passwords/set")]
        [AllowAnonymous]
        public async Task<Response<UserDTO>> SetPassword([FromBody] UserResetPasswordDTO dto)
        {
            return await _handler.ResetPassword(dto);
        }
        
        /// <summary>
        /// Send link to email for restore password
        /// </summary>
        [HttpPost("passwords/request")]
        [AllowAnonymous]
        public async Task<Response> ResetPasswordRequest([FromBody, Required] UserEmailDTO dto)
        {
            return await _handler.ResetPasswordRequest(dto.Email);
        }
    }
}