using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Squadio.API.Handlers.Companies;
using Squadio.API.Handlers.Invites;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Enums;
using Squadio.DTO.Companies;
using Squadio.DTO.Invites;
using Squadio.DTO.Users;

namespace Squadio.API.Controllers
{
    [ApiController]
    //[AuthorizationFilter]
    [Route("api/company")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompaniesHandler _handler;
        private readonly IInvitesHandler _invitesHandler;

        public CompanyController(ICompaniesHandler handler
            , IInvitesHandler invitesHandler)
        {
            _handler = handler;
            _invitesHandler = invitesHandler;
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
            return await _invitesHandler.InviteToCompany(id, dto, User);
        }
        /// <summary>
        /// Cancel of invites to company
        /// </summary>
        [HttpPut("{id}/invite/cancel")]
        public async Task<Response> CancelInvite([Required, FromRoute] Guid id
            , [Required, FromBody] CancelInvitesDTO dto)
        {
            return await _invitesHandler.CancelInvite(id, dto, User, EntityType.Company);
        }
        
        /// <summary>
        /// Remove user from company
        /// </summary>
        [HttpDelete("{companyId}/user/{userId}")]
        public Task<Response> DeleteCompanyUser([Required, FromRoute] Guid companyId, [Required, FromRoute] Guid userId)
        {
            return _handler.DeleteCompanyUser(companyId, userId, User);
        }
        
        /// <summary>
        /// Accept invite to company
        /// </summary>
        [HttpPost("invite/accept")]
        public async Task<Response> AcceptInvite([Required, FromQuery] string code)
        {
            return await _invitesHandler.AcceptInvite(User, code, EntityType.Company);
        }
    }
}