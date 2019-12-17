﻿using System.Threading.Tasks;
using Squadio.Common.WebSocket;

namespace Squadio.API.WebSocketHubHandlers.Projects
{
    public interface ISidebarHubHandler
    {
        Task BroadcastSidebarChanges(BroadcastChangesModel model);
    }
}