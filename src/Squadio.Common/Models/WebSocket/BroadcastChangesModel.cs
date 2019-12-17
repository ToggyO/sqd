using System;
using Squadio.Common.Enums;

namespace Squadio.Common.WebSocket
{
    public class BroadcastChangesModel
    {
        public Guid EntityId { get; set; }
        public ConnectionGroup ConnectionGroup { get; set; }
        public EntityType EntityType { get; set; }
    }
}