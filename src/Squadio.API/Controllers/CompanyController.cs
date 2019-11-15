using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Squadio.API.Filters;
using Squadio.API.Handlers.Companies;
using Squadio.API.Handlers.Invites;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Companies;
using Squadio.DTO.Invites;
using Squadio.DTO.Users;

namespace Squadio.API.Controllers
{
    [ApiController]
    [AuthorizationFilter]
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

        [HttpGet]
        public async Task<Response<PageModel<CompanyDTO>>> GetCompanies([FromQuery] PageModel model)
        {
            return await _handler.GetCompanies(model);
        }
        
        [HttpGet("{id}")]
        public Task<Response<CompanyDTO>> GetCompany([Required, FromRoute] Guid id)
        {
            return _handler.GetCompany(id);
        }
        
        [HttpGet("{id}/users")]
        public Task<Response<PageModel<CompanyUserDTO>>> GetCompanyUsers([Required, FromRoute] Guid id
            , [FromQuery] PageModel model)
        {
            return _handler.GetCompanyUsers(id, model);
        }
        
        [HttpPost]
        public Task<Response<CompanyDTO>> CreateCompany([Required, FromBody] CreateCompanyDTO dto)
        {
            return _handler.CreateCompany(dto, User);
        }
        
        [HttpPut("{id}")]
        public Task<Response<CompanyDTO>> UpdateCompany([Required, FromRoute] Guid id, [Required, FromBody] CompanyUpdateDTO dto)
        {
            return _handler.UpdateCompany(id, dto, User);
        }
        
        [HttpPost("{id}/invite")]
        public async Task<Response<IEnumerable<InviteDTO>>> CreateInvites([Required, FromRoute] Guid id
            , [Required, FromBody] CreateInvitesDTO dto)
        {
            return await _invitesHandler.InviteToCompany(id, dto, User);
        }
        
        [HttpPut("{id}/invite/cancel")]
        public async Task<Response> CancelInvite([Required, FromRoute] Guid id
            , [Required, FromBody] CancelInvitesDTO dto)
        {
            return await _invitesHandler.CancelInvite(id, dto, User);
        }
        
        [HttpDelete("{companyId}/user/{userId}")]
        public Task<Response> DeleteCompanyUser([Required, FromRoute] Guid companyId, [Required, FromRoute] Guid userId)
        {
            return _handler.DeleteCompanyUser(companyId, userId, User);
        }
    }
}