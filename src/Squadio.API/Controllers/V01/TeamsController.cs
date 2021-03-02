using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Squadio.API.Handlers.Teams;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Models.Invites;
using Squadio.DTO.Models.Teams;
using Squadio.DTO.Models.Users;

namespace Squadio.API.Controllers.V01
{
    [ApiController]
    //[AuthorizationFilter]
    [ApiVersion("0.1")]
    [Route("api/v{version:apiVersion}/teams")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamsHandler _handler;
        
        public TeamsController(ITeamsHandler handler)
        {
            _handler = handler;
        }
        
        /// <summary>
        /// Get teams with pagination
        /// </summary>
        [HttpGet]
        public async Task<Response<PageModel<TeamDTO>>> GetTeams([FromQuery] PageModel model,
            [FromQuery] Guid? companyId)
        {
            return await _handler.GetTeams(model, companyId);
        }
        
        /// <summary>
        /// Get team by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<Response<TeamDTO>> GetTeam([Required, FromRoute] Guid id)
        {
            return await _handler.GetById(id);
        }
        
        /// <summary>
        /// Update team by id
        /// </summary>
        [HttpPut("{id}")]
        public async Task<Response<TeamDTO>> UpdateTeam([Required, FromRoute] Guid id, [Required, FromBody] TeamUpdateDTO dto)
        {
            return await _handler.Update(id, dto, User);
        }
        
        /// <summary>
        /// Delete team by id
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<Response<TeamDTO>> DeleteTeam([Required, FromRoute] Guid id)
        {
            return await _handler.Delete(id, User);
        }
        
        /// <summary>
        /// Delete user from team
        /// </summary>
        [HttpDelete("{teamId}/user/{userId}")]
        public Task<Response> DeleteTeamUser([Required, FromRoute] Guid teamId, [Required, FromRoute] Guid userId)
        {
            return _handler.DeleteTeamUser(teamId, userId, User);
        }
        
        /// <summary>
        /// Delete current user from team
        /// </summary>
        [HttpDelete("{teamId}/leave")]
        public Task<Response> LeaveTeam([Required, FromRoute] Guid teamId)
        {
            return _handler.LeaveTeam(teamId, User);
        }
        
        /// <summary>
        /// Get users of team
        /// </summary>
        [HttpGet("{id}/users")]
        public Task<Response<PageModel<UserWithRoleDTO>>> GetTeamUsers([Required, FromRoute] Guid id
            , [FromQuery] PageModel model)
        {
            return _handler.GetTeamUsers(id, model);
        }
        
        /// <summary>
        /// Create new team
        /// </summary>
        [HttpPost]
        public async Task<Response<TeamDTO>> Create([FromQuery, Required] Guid companyId, [FromBody] TeamCreateDTO dto)
        {
            return await _handler.Create(companyId, dto, User);
        }
        
        /// <summary>
        /// Send invites to team
        /// </summary>
        [HttpPost("{id}/invite")]
        public async Task<Response> CreateInvites([Required, FromRoute] Guid id
            , [Required, FromBody] CreateInvitesDTO dto)
        {
            return await _handler.InviteTeamUsers(id, dto, User);
        }
    }
}