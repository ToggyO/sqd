using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Squadio.API.WebSocketHubs;
using Squadio.BLL.Providers.Projects;
using Squadio.BLL.Providers.Teams;
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
        private readonly ITeamsProvider _teamsProvider;
        private readonly IHubContext<SidebarHub> _hub;

        public SidebarHubHandler(ILogger<SidebarHubHandler> logger
            , IProjectsProvider projectsProvider
            , ITeamsProvider teamsProvider
            , IHubContext<SidebarHub> hub)
        {
            _logger = logger;
            _projectsProvider = projectsProvider;
            _teamsProvider = teamsProvider;
            _hub = hub;
        }

        public async Task BroadcastSidebarChanges(BroadcastSidebarChangesModel model)
        {
            try
            {
                if (Guid.TryParse(model.TeamId, out var teamGuid))
                {
                    var projectsResponsePage = await _projectsProvider.GetProjects(
                        new PageModel { Page = 1, PageSize = 10000 }, 
                        new ProjectFilter { TeamId = teamGuid });
                    var projectsPage = projectsResponsePage.Data;
                    var projects = projectsPage.Items;

                    var usersPageResponse = await _teamsProvider.GetTeamUsers(
                        teamGuid, 
                        new PageModel { Page = 1, PageSize = 10000 });
                    var usersPage = usersPageResponse.Data;
                    var users = usersPage.Items;
                    
                    foreach (var user in users)
                    {
                        var projectsOfUser = projects.Where(x=>x.)
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