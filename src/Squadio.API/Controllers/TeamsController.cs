using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Squadio.API.Handlers.Teams;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Teams;

namespace Squadio.API.Controllers
{
    [ApiController]
    [Route("api/teams")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamsHandler _handler;
        public TeamsController(ITeamsHandler handler)
        {
            _handler = handler;
        }
        
        [HttpGet]
        public string GetTeam()
        {
            return "Get from team controller";
        }
        
        [HttpGet("{id}")]
        public async Task<Response<TeamDTO>> GetTeam([Required, FromRoute] Guid id)
        {
            return await _handler.GetById(id);
        }
        
        [HttpPost]
        public async Task<Response<TeamDTO>> Create([FromBody] CreateTeamDTO dto)
        {
            return await _handler.Create(dto, User);
        }
    }
}