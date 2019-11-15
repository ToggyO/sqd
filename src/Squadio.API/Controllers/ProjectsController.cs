using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Squadio.API.Filters;
using Squadio.API.Handlers.Invites;
using Squadio.API.Handlers.Projects;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Invites;
using Squadio.DTO.Projects;
using Squadio.DTO.Users;

namespace Squadio.API.Controllers
{
    [ApiController]
    //[AuthorizationFilter]
    [Route("api/projects")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectsHandler _handler;
        private readonly IInvitesHandler _invitesHandler;
        public ProjectsController(IProjectsHandler handler
            , IInvitesHandler invitesHandler)
        {
            _handler = handler;
            _invitesHandler = invitesHandler;
        }
        
        [HttpGet]
        public async Task<Response<PageModel<ProjectDTO>>> GetProjects([FromQuery] PageModel model)
        {
            return await _handler.GetProjects(model);
        }
        
        [HttpGet("{id}")]
        public async Task<Response<ProjectDTO>> GetProject([Required, FromRoute] Guid id)
        {
            return await _handler.GetById(id);
        }
        
        [HttpPut("{id}")]
        public async Task<Response<ProjectDTO>> UpdateTeam([Required, FromRoute] Guid id, [Required, FromBody] ProjectUpdateDTO dto)
        {
            return await _handler.Update(id, dto, User);
        }
        
        [HttpDelete("{projectId}/user/{userId}")]
        public Task<Response> DeleteProjectUser([Required, FromRoute] Guid projectId, [Required, FromRoute] Guid userId)
        {
            return _handler.DeleteProjectUser(projectId, userId, User);
        }
        
        [HttpGet("{id}/users")]
        public Task<Response<PageModel<ProjectUserDTO>>> GetProjectUsers([Required, FromRoute] Guid id
            , [FromQuery] PageModel model)
        {
            return _handler.GetProjectUsers(id, model);
        }
        
        [HttpPost]
        public async Task<Response<ProjectDTO>> Create([FromQuery, Required] Guid teamId, [FromBody] CreateProjectDTO dto)
        {
            return await _handler.Create(teamId, dto, User);
        }
        
        [HttpPost("{id}/invite")]
        public async Task<Response<IEnumerable<InviteDTO>>> CreateInvites([Required, FromRoute] Guid id
            , [Required, FromBody] CreateInvitesDTO dto)
        {
            return await _invitesHandler.InviteToProject(id, dto, User);
        }
        
        [HttpPost("invite/accept")]
        public async Task<Response> AcceptInvite([Required, FromQuery] string code)
        {
            return await _invitesHandler.AcceptInvite(User, code);
        }
    }
}