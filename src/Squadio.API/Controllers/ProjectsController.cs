using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Squadio.API.Handlers.Projects;
using Squadio.API.Handlers.Teams;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Teams;

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
    }
}