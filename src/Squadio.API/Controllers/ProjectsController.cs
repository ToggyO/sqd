using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Squadio.API.Handlers.Projects;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Projects;

namespace Squadio.API.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectsHandler _handler;
        public ProjectsController(IProjectsHandler handler)
        {
            _handler = handler;
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
    }
}