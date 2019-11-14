using Microsoft.AspNetCore.Mvc;
using Squadio.API.Handlers.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Squadio.API.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Companies;
using Squadio.DTO.Projects;
using Squadio.DTO.Teams;
using Squadio.DTO.Users;

namespace Squadio.API.Controllers
{
    [ApiController]
    [AuthorizationFilter]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersHandler _handler;

        public UsersController(IUsersHandler handler)
        {
            _handler = handler;
        }
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<Response<PageModel<UserDTO>>> GetPage([FromQuery] PageModel model)
        {
            return await _handler.GetPage(model);
        }
        
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<Response<UserDTO>> DeleteUser([Required, FromRoute] Guid id)
        {
            return await _handler.DeleteUser(id);
        }
        
        [HttpGet("{id}")]
        public async Task<Response<UserDTO>> GetById([Required, FromRoute] Guid id)
        {
            return await _handler.GetById(id);
        }
        
        [HttpGet("{id}/companies")]
        public async Task<Response<PageModel<CompanyUserDTO>>> GetUserCompanies([Required, FromRoute] Guid id, [FromQuery] PageModel model)
        {
            return await _handler.GetUserCompanies(id, model);
        }
        
        [HttpGet("{id}/teams")]
        public async Task<Response<PageModel<TeamUserDTO>>> GetUserTeams([Required, FromRoute] Guid id, [FromQuery] PageModel model)
        {
            return await _handler.GetUserTeams(id, model);
        }
        
        [HttpGet("{id}/projects")]
        public async Task<Response<PageModel<ProjectUserDTO>>> GetUserProjects([Required, FromRoute] Guid id, [FromQuery] PageModel model)
        {
            return await _handler.GetUserProjects(id, model);
        }
        
        [HttpGet("current")]
        public async Task<Response<UserDTO>> GetCurrentUser()
        {
            return await _handler.GetCurrentUser(User);
        }
        
        [HttpGet("current/companies")]
        public async Task<Response<PageModel<CompanyUserDTO>>> GetUserCompanies([FromQuery] PageModel model)
        {
            return await _handler.GetUserCompanies(User, model);
        }
        
        [HttpGet("current/teams")]
        public async Task<Response<PageModel<TeamUserDTO>>> GetUserTeams([FromQuery] PageModel model)
        {
            return await _handler.GetUserTeams(User, model);
        }
        
        [HttpGet("current/projects")]
        public async Task<Response<PageModel<ProjectUserDTO>>> GetUserProjects([FromQuery] PageModel model)
        {
            return await _handler.GetUserProjects(User, model);
        }
        
        [HttpPut("current")]
        public async Task<Response<UserDTO>> UpdateCurrentUser([FromBody] UserUpdateDTO dto)
        {
            return await _handler.UpdateCurrentUser(dto, User);
        }
        
        [HttpPut("passwords/set")]
        [AllowAnonymous]
        public async Task<Response<UserDTO>> SetPassword([FromBody] UserResetPasswordDTO dto)
        {
            return await _handler.ResetPassword(dto);
        }
        
        [HttpPost("passwords/request")]
        [AllowAnonymous]
        public async Task<Response> ResetPasswordRequest([FromBody, Required] UserEmailDTO dto)
        {
            return await _handler.ResetPasswordRequest(dto.Email);
        }
        
        [HttpPut("email/set")]
        public async Task<Response<UserDTO>> SetEmail([FromBody] UserSetEmailDTO dto)
        {
            return await _handler.SetEmail(dto, User);
        }
        
        [HttpPost("email/request")]
        public async Task<Response> ChangeEmailRequest([FromBody, Required] ChangeEmailRequestDTO dto)
        {
            return await _handler.ChangeEmailRequest(dto, User);
        }
    }
}