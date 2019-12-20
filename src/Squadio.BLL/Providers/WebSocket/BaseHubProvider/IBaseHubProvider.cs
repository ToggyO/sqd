﻿using System;
using System.Threading.Tasks;
using Squadio.Common.Enums;

namespace Squadio.BLL.Providers.WebSocket.BaseHubProvider
{
    public interface IBaseHubProvider
    {
        Task BroadcastChanges(Guid groupId, ConnectionGroup connectionGroup, string methodName, object model);
    }
}