using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Squadio.API.Handlers.Companies;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Companies;

namespace Squadio.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/company")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompaniesHandler _handler;

        public CompanyController(ICompaniesHandler handler)
        {
            _handler = handler;
        }
        
        [HttpGet("{id}")]
        public Task<Response<CompanyDTO>> GetCompany([Required, FromRoute] Guid id)
        {
            return _handler.GetCompany(id);
        }
        
        [HttpPost]
        public Task<Response<CompanyDTO>> CreateCompany([Required, FromBody] CreateCompanyDTO dto)
        {
            return _handler.CreateCompany(dto, User);
        }
    }
}