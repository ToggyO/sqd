using System.Collections.Generic;
using System.Linq;

namespace Squadio.Common.WebSocket
{
    public class ConnectionDictionary<T>
    {
        private ConnectionDictionary(){}
        private static readonly ConnectionDictionary<T> _instance = new ConnectionDictionary<T>();
        public static ConnectionDictionary<T> GetInstance() => _instance;
        
        private readonly Dictionary<T, HashSet<string>> _connections =
            new Dictionary<T, HashSet<string>>();
        
        public void Add(T key, string connectionId)
        {
            lock (_connections)
            {
                if (!_connections.TryGetValue(key,out var connId))
                {
                    connId = new HashSet<string>();
                    _connections.Add(key, connId);
                }
                lock (connId)
                {
                    connId.Add(connectionId);
                }
            }
        }
        
        public void Remove(T key, string connectionId)
        {
            lock (_connections)
            {
                if(!_connections.TryGetValue(key,out var connId))
                { return; }
                lock (connId)
                {
                    connId.Remove(connectionId);
                    if (connId.Count == 0)
                    { _connections.Remove(key); }
                }
            }
        }
        
        public IEnumerable<string> GetUserConnections(T key)
        {
            lock (_connections)
            {
                if (_connections.TryGetValue(key,out var connId))
                {
                    return connId;
                }
            }

            return Enumerable.Empty<string>();
        }
    }
}