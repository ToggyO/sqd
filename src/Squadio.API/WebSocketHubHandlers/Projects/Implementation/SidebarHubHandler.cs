using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Squadio.API.WebSocketHubs;
using Squadio.BLL.Providers.Projects;
using Squadio.Common.Enums;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.WebSocket;

namespace Squadio.API.WebSocketHubHandlers.Projects.Implementation
{
    public class SidebarHubHandler : ISidebarHubHandler
    {
        private readonly ILogger<SidebarHubHandler> _logger;
        private readonly IProjectsProvider _projectsProvider;
        private readonly IHubContext<SidebarHub> _hub;
        private readonly GroupUsersDictionary<Guid> _groupUsers = GroupUsersDictionary<Guid>.GetInstance();

        public SidebarHubHandler(ILogger<SidebarHubHandler> logger
            , IProjectsProvider projectsProvider
            , IHubContext<SidebarHub> hub)
        {
            _logger = logger;
            _projectsProvider = projectsProvider;
            _hub = hub;
        }

        public async Task BroadcastSidebarChanges(BroadcastSidebarChangesModel model)
        {
            try
            {
                if (Guid.TryParse(model.TeamId, out var teamGuid))
                {
                    var projectsResponsePage = await _projectsProvider.GetProjects(new PageModel(), new ProjectFilter
                    {
                        TeamId = teamGuid
                    });

                    var users = _groupUsers.GetUserIds(teamGuid);
                    var projectsPage = projectsResponsePage.Data;
                    foreach (var user in users)
                    {
                        var group = _hub.Clients.Groups(user.ToString());
                        await group.SendAsync("BroadcastProjects", projectsPage);
                    }
                    
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }
    }
}