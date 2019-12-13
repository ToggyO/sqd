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
                    var projectUsersResponsePage = await _projectsProvider.GetProjectUsers(
                        new PageModel { Page = 1, PageSize = 100000 },
                        teamId: teamGuid);
                    var projectUsersPage = projectUsersResponsePage.Data;
                    var projectUsers = projectUsersPage.Items.ToList();
                    var userIds = projectUsers.Select(x => x.User.Id).Distinct();
                    
                    foreach (var userId in userIds)
                    {
                        var projects = projectUsers
                            .Where(x => x.UserId == userId)
                            .Select(x => x.Project)
                            .Distinct();
                        var group = _hub.Clients.Groups(userId.ToString());
                        await group.SendAsync("BroadcastProjects", projects);
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