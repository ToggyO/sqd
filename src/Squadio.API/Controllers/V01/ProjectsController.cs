using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Squadio.API.Handlers.Projects;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Models.Invites;
using Squadio.DTO.Models.Projects;
using Squadio.DTO.Models.Users;
using Squadio.DTO.Projects;

namespace Squadio.API.Controllers.V01
{
    [ApiController]
    [ApiVersion("0.1")]
    [Route("v{version:apiVersion}/projects")]
    //TODO: auth filter
    //[ServiceFilter(typeof(AuthorizationFilter))]
    // [ServiceFilter(typeof(UserStatusFilter))]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectsHandler _handler;
        public ProjectsController(IProjectsHandler handler)
        {
            _handler = handler;
        }
        
        /// <summary>
        /// Get projects with pagination
        /// </summary>
        [HttpGet]
        public async Task<Response<PageModel<ProjectDTO>>> GetProjects([FromQuery] PageModel model
            , [FromQuery] Guid? companyId
            , [FromQuery] Guid? teamId)
        {
            return await _handler.GetProjects(model, companyId, teamId);
        }
        
        /// <summary>
        /// Get project by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<Response<ProjectDTO>> GetProject([Required, FromRoute] Guid id)
        {
            return await _handler.GetById(id);
        }
        
        /// <summary>
        /// Update project by id
        /// </summary>
        [HttpPut("{id}")]
        public async Task<Response<ProjectDTO>> UpdateProject([Required, FromRoute] Guid id, [Required, FromBody] ProjectUpdateDTO dto)
        {
            return await _handler.Update(id, dto, User);
        }
        
        /// <summary>
        /// Delete project by id
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<Response> DeleteProject([Required, FromRoute] Guid id)
        {
            return await _handler.Delete(id, User);
        }
        
        /// <summary>
        /// Delete user from project
        /// </summary>
        [HttpDelete("{projectId}/user/{userId}")]
        public Task<Response> DeleteProjectUser([Required, FromRoute] Guid projectId, [Required, FromRoute] Guid userId)
        {
            return _handler.DeleteProjectUser(projectId, userId, User);
        }
        
        /// <summary>
        /// Get users of project
        /// </summary>
        [HttpGet("{id}/users")]
        public Task<Response<PageModel<UserWithRoleDTO>>> GetProjectUsers([Required, FromRoute] Guid id
            , [FromQuery] PageModel model)
        {
            return _handler.GetProjectUsers(id, model);
        }
        
        /// <summary>
        /// Create new project
        /// </summary>
        [HttpPost]
        public async Task<Response<ProjectDTO>> Create([FromQuery, Required] Guid teamId, [FromBody] ProjectCreateDTO dto)
        {
            return await _handler.Create(teamId, dto, User);
        }
        
        /// <summary>
        /// Send invites to project
        /// </summary>
        [HttpPost("{id}/invite")]
        public async Task<Response> CreateInvites([Required, FromRoute] Guid id
            , [Required, FromBody] CreateInvitesDTO dto)
        {
            return await _handler.InviteProjectUsers(id, dto, User);
        }
    }
}