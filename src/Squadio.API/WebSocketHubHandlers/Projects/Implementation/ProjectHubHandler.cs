using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Squadio.API.WebSocketHubs;
using Squadio.BLL.Providers.Projects;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.WebSocket;

namespace Squadio.API.WebSocketHubHandlers.Projects.Implementation
{
    public class ProjectHubHandler : IProjectHubHandler
    {
        private readonly ILogger<ProjectHubHandler> _logger;
        private readonly IProjectsProvider _projectsProvider;
        private readonly IHubContext<ProjectHub> _hub;

        public ProjectHubHandler(ILogger<ProjectHubHandler> logger
            , IProjectsProvider projectsProvider
            , IHubContext<ProjectHub> hub)
        {
            _logger = logger;
            _projectsProvider = projectsProvider;
            _hub = hub;
        }

        public async Task BroadcastTeamChanges(BroadcastTeamChangesModel model)
        {
            try
            {
                if (Guid.TryParse(model.TeamId, out var teamGuid))
                {
                    var projectsResponsePage = await _projectsProvider.GetProjects(new PageModel(), new ProjectFilter
                    {
                        TeamId = teamGuid
                    });
                    var projectsPage = projectsResponsePage.Data;
                    var group = _hub.Clients.Groups(model.TeamId);
                    await group.SendAsync("BroadcastProjects", projectsPage);
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