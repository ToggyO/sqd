using System;
using Squadio.Common.Models.WebSocket;

namespace Squadio.Common.WebSocket
{
    public class BroadcastSidebarProjectChangesModel : BaseChangesModel
    {
        public Guid TeamId { get; set; }
        public Guid ProjectId { get; set; }
    }
}