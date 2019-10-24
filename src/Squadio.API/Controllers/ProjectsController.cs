using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Squadio.API.Filters;
using Squadio.API.Handlers.Invites;
using Squadio.API.Handlers.Projects;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Invites;
using Squadio.DTO.Pages;
using Squadio.DTO.Projects;
using Squadio.DTO.Users;

namespace Squadio.API.Controllers
{
    [ApiController]
    [AuthorizationFilter]
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
        
        [HttpGet("{id}")]
        public async Task<Response<ProjectDTO>> GetProject([Required, FromRoute] Guid id)
        {
            return await _handler.GetById(id);
        }
        
        [HttpGet("{id}/users")]
        public Task<Response<PageModel<UserDTO>>> GetProjectUsers([Required, FromRoute] Guid id
            , [FromQuery] PageModel model)
        {
            return _handler.GetProjectUsers(id, model);
        }
        
        [HttpPost]
        public async Task<Response<ProjectDTO>> Create([FromBody] CreateProjectDTO dto)
        {
            return await _handler.Create(dto, User);
        }
        
        [HttpPost("{id}/invite")]
        public async Task<Response<IEnumerable<InviteDTO>>> CreateInvites([Required, FromRoute] Guid id
            , [Required, FromBody] CreateInvitesDTO dto)
        {
            return await _invitesHandler.InviteToProject(id, dto, User);
        }
        
        [HttpPost("{id}/invite/accept")]
        public async Task<Response> AcceptInvite([Required, FromRoute] Guid id
            , [Required, FromQuery] string code)
        {
            return await _invitesHandler.AcceptInviteToProject(id, User, code);
        }
    }
}