using System.Threading.Tasks;
using Squadio.Common.WebSocket;

namespace Squadio.API.WebSocketHubHandlers.Projects
{
    public interface IProjectHubHandler
    {
        Task BroadcastTeamChanges(BroadcastTeamChangesModel model);
    }
}