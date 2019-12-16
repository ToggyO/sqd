using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Internal;
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
        private readonly IHubContext<SidebarHub> _hub;
        private readonly GroupUsersDictionary<Guid> _dictionary;

        public SidebarHubHandler(ILogger<SidebarHubHandler> logger
            , IProjectsProvider projectsProvider
            , IHubContext<SidebarHub> hub)
        {
            _logger = logger;
            _projectsProvider = projectsProvider;
            _hub = hub;
            _dictionary = GroupUsersDictionary<Guid>.GetInstance();
        }

        public async Task BroadcastSidebarChanges(BroadcastSidebarChangesModel model)
        {
            try
            {
                if (model != null && model?.TeamId != Guid.Empty)
                {
                    var userIds = _dictionary.GetUsers(model.TeamId);
                    var projectUsersResponsePage = await _projectsProvider.GetProjectUsers(
                        new PageModel { Page = 1, PageSize = 100000 },
                        teamId: model.TeamId);
                    var projectUsersPage = projectUsersResponsePage.Data;
                    var projectUsers = projectUsersPage.Items.ToList();
                    
                    foreach (var userId in userIds)
                    {
                        var projects = projectUsers
                            .Where(x => x.UserId == userId)
                            .Select(x => x.Project)
                            .Distinct();
                        var connections = _dictionary.GetConnections(model.TeamId, userId);
                        await _hub.Clients.Clients(connections).SendAsync("BroadcastProjects", projects);
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