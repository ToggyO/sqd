using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Squadio.API.Handlers.Invites;
using Squadio.API.Handlers.Projects;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Invites;
using Squadio.DTO.Projects;

namespace Squadio.API.Controllers
{
    [ApiController]
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
    }
}