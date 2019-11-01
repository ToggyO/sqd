using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Squadio.API.Filters;
using Squadio.API.Handlers.Invites;
using Squadio.API.Handlers.Teams;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Invites;
using Squadio.DTO.Teams;
using Squadio.DTO.Users;

namespace Squadio.API.Controllers
{
    [ApiController]
    //[AuthorizationFilter]
    [Route("api/teams")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamsHandler _handler;
        private readonly IInvitesHandler _invitesHandler;
        public TeamsController(ITeamsHandler handler
            , IInvitesHandler invitesHandler)
        {
            _handler = handler;
            _invitesHandler = invitesHandler;
        }
        [HttpGet]
        public async Task<Response<PageModel<TeamDTO>>> GetTeams([FromQuery] PageModel model)
        {
            return await _handler.GetTeams(model);
        }
        
        [HttpGet("{id}")]
        public async Task<Response<TeamDTO>> GetTeam([Required, FromRoute] Guid id)
        {
            return await _handler.GetById(id);
        }
        
        [HttpGet("{id}/users")]
        public Task<Response<PageModel<TeamUserDTO>>> GetTeamUsers([Required, FromRoute] Guid id
            , [FromQuery] PageModel model)
        {
            return _handler.GetTeamUsers(id, model);
        }
        
        [HttpPost]
        public async Task<Response<TeamDTO>> Create([FromQuery, Required] Guid companyId, [FromBody] CreateTeamDTO dto)
        {
            return await _handler.Create(companyId, dto, User);
        }
        
        [HttpPost("{id}/invite")]
        public async Task<Response<IEnumerable<InviteDTO>>> CreateInvites([Required, FromRoute] Guid id
            , [Required, FromBody] CreateInvitesDTO dto)
        {
            return await _invitesHandler.InviteToTeam(id, dto, User);
        }
        
        [HttpPost("{id}/invite/accept")]
        public async Task<Response> AcceptInvite([Required, FromRoute] Guid id
            , [Required, FromQuery] string code)
        {
            return await _invitesHandler.AcceptInviteToTeam(id, User, code);
        }
    }
}