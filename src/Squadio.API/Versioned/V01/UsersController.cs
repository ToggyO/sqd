using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Squadio.API.Filters;
using Squadio.API.Handlers.Users;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Auth;
using Squadio.DTO.Companies;
using Squadio.DTO.Projects;
using Squadio.DTO.Resources;
using Squadio.DTO.Teams;
using Squadio.DTO.Users;
using Squadio.DTO.Users.Settings;

namespace Squadio.API.Versioned.V01
{
    [ApiController]
    [AuthorizationFilter]
    [ApiVersion("0.1")]
    [Route("api/v{version:apiVersion}/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersHandler _handler;
        private readonly IUsersSettingsHandler _settingsHandler;

        public UsersController(IUsersHandler handler
            , IUsersSettingsHandler settingsHandler)
        {
            _handler = handler;
            _settingsHandler = settingsHandler;
        }
        
        /// <summary>
        /// Get users with pagination (for development purposes)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<Response<PageModel<UserDTO>>> GetPage([FromQuery] PageModel model)
        {
            return await _handler.GetPage(model);
        }
        
        /// <summary>
        /// Delete user by id (for development purposes)
        /// </summary>
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<Response<UserDTO>> DeleteUser([Required, FromRoute] Guid id)
        {
            return await _handler.DeleteUser(id);
        }
        
        /// <summary>
        /// Get user by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<Response<UserDTO>> GetById([Required, FromRoute] Guid id)
        {
            return await _handler.GetById(id);
        }

        /// <summary>
        /// Get companies of user
        /// </summary>
        [HttpGet("{id}/companies")]
        public async Task<Response<PageModel<CompanyWithUserRoleDTO>>> GetUserCompanies([Required, FromRoute] Guid id
            , [FromQuery] PageModel model
            , [FromQuery] Guid? companyId)
        {
            return await _handler.GetUserCompanies(id, model, companyId);
        }
        
        /// <summary>
        /// Get teams of user
        /// </summary>
        [HttpGet("{id}/teams")]
        public async Task<Response<PageModel<TeamWithUserRoleDTO>>> GetUserTeams([Required, FromRoute] Guid id
            , [FromQuery] PageModel model
            , [FromQuery] Guid? companyId
            , [FromQuery] Guid? teamId)
        {
            return await _handler.GetUserTeams(id, model, companyId, teamId);
        }
        
        /// <summary>
        /// Get projects of user
        /// </summary>
        [HttpGet("{id}/projects")]
        public async Task<Response<PageModel<ProjectWithUserRoleDTO>>> GetUserProjects([Required, FromRoute] Guid id
            , [FromQuery] PageModel model
            , [FromQuery] Guid? companyId
            , [FromQuery] Guid? teamId
            , [FromQuery] Guid? projectId)
        {
            return await _handler.GetUserProjects(id, model, companyId, teamId, projectId);
        }
        
        /// <summary>
        /// Get current user
        /// </summary>
        [HttpGet("current")]
        public async Task<Response<UserDTO>> GetCurrentUser()
        {
            return await _handler.GetCurrentUser(User);
        }
        
        /// <summary>
        /// Get companies current user
        /// </summary>
        [HttpGet("current/companies")]
        public async Task<Response<PageModel<CompanyWithUserRoleDTO>>> GetUserCompanies([FromQuery] PageModel model
            , [FromQuery] Guid? companyId)
        {
            return await _handler.GetUserCompanies(User, model, companyId);
        }
        
        /// <summary>
        /// Get teams current user
        /// </summary>
        [HttpGet("current/teams")]
        public async Task<Response<PageModel<TeamWithUserRoleDTO>>> GetUserTeams([FromQuery] PageModel model
            , [FromQuery] Guid? companyId
            , [FromQuery] Guid? teamId)
        {
            return await _handler.GetUserTeams(User, model, companyId, teamId);
        }
        
        /// <summary>
        /// Get projects current user
        /// </summary>
        [HttpGet("current/projects")]
        public async Task<Response<PageModel<ProjectWithUserRoleDTO>>> GetUserProjects([FromQuery] PageModel model
            , [FromQuery] Guid? companyId
            , [FromQuery] Guid? teamId
            , [FromQuery] Guid? projectId)
        {
            return await _handler.GetUserProjects(User, model, companyId, teamId, projectId);
        }
    }
}