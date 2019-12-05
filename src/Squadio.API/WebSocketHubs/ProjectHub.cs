using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Squadio.API.Handlers.Projects;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;

namespace Squadio.API.WebSocketHubs
{
    [Authorize]
    public class ProjectHub : Hub
    {
        private readonly ILogger<ProjectHub> _logger;
        private readonly IProjectsHandler _handler;
        
        public ProjectHub(ILogger<ProjectHub> logger
            , IProjectsHandler handler)
        {
            _logger = logger;
            _handler = handler;
            _logger.LogInformation("ProjectsHub constructor work done");
        }
        
        [HubMethodName("AddToGroup")]
        public async Task AddToGroup(AddToGroupDTO model)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, model.TeamId);
        }
        
        [HubMethodName("RemoveFromGroup")]
        public async Task RemoveFromGroup(RemoveFromGroupDTO model)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, model.TeamId);
        }
        
        [HubMethodName("SendProjects")]
        public async Task SendProjects(SendProjectsDTO model)
        {
            if (Guid.TryParse(model.TeamId, out var teamGuid))
            {
                var projectsResponsePage = await _handler.GetProjects(new PageModel(), new ProjectFilter
                {
                    TeamId = teamGuid
                });
                var projectsPage = projectsResponsePage.Data;
                var group = Clients.Groups(model.TeamId);
                await group.SendAsync("BroadcastProjects", projectsPage);
            }
        }
    }

    public class BaseWS
    {
        public string Token { get; set; }
    }

    public class SendProjectsDTO : BaseWS
    {
        public string TeamId { get; set; }
    }

    public class AddToGroupDTO : BaseWS
    {
        public string TeamId { get; set; }
    }

    public class RemoveFromGroupDTO : BaseWS
    {
        public string TeamId { get; set; }
    }
}