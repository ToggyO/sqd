using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Squadio.API.Handlers.Companies;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Models.Companies;
using Squadio.DTO.Models.Invites;
using Squadio.DTO.Models.Users;

namespace Squadio.API.Controllers.V01
{
    [ApiController]
    [ApiVersion("0.1")]
    [Route("v{version:apiVersion}/company")]
    //TODO: auth filter
    //[ServiceFilter(typeof(AuthorizationFilter))]
    // [ServiceFilter(typeof(UserStatusFilter))]
    public class CompanyController : ControllerBase
    {
        private readonly ICompaniesHandler _handler;

        public CompanyController(ICompaniesHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Get companies with pagination
        /// </summary>
        [HttpGet]
        public async Task<Response<PageModel<CompanyDTO>>> GetCompanies([FromQuery] PageModel model)
        {
            return await _handler.GetCompanies(model);
        }
        
        /// <summary>
        /// Get company by id
        /// </summary>
        [HttpGet("{id}")]
        public Task<Response<CompanyDTO>> GetCompany([Required, FromRoute] Guid id)
        {
            return _handler.GetCompany(id);
        }
        
        /// <summary>
        /// Get users of company
        /// </summary>
        [HttpGet("{id}/users")]
        public Task<Response<PageModel<UserWithRoleDTO>>> GetCompanyUsers([Required, FromRoute] Guid id
            , [FromQuery] PageModel model)
        {
            return _handler.GetCompanyUsers(id, model);
        }
        
        /// <summary>
        /// Create new company
        /// </summary>
        [HttpPost]
        public Task<Response<CompanyDTO>> CreateCompany([Required, FromBody] CompanyCreateDTO dto)
        {
            return _handler.CreateCompany(dto, User);
        }
        
        /// <summary>
        /// Update company by id
        /// </summary>
        [HttpPut("{id}")]
        public Task<Response<CompanyDTO>> UpdateCompany([Required, FromRoute] Guid id, [Required, FromBody] CompanyUpdateDTO dto)
        {
            return _handler.UpdateCompany(id, dto, User);
        }
        
        /// <summary>
        /// Send invites to company
        /// </summary>
        [HttpPost("{id}/invite")]
        public async Task<Response> CreateInvites([Required, FromRoute] Guid id
            , [Required, FromBody] CreateInvitesDTO dto)
        {
            return await _handler.InviteCompanyUsers(id, dto, User);
        }
        
        /// <summary>
        /// Remove user from company
        /// </summary>
        [HttpDelete("{companyId}/user/{userId}")]
        public Task<Response> DeleteCompanyUser([Required, FromRoute] Guid companyId, [Required, FromRoute] Guid userId)
        {
            return _handler.DeleteCompanyUser(companyId, userId, User);
        }
    }
}