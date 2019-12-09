using System;
using System.Collections.Generic;
using System.Linq;
using Squadio.Common.Enums;

namespace Squadio.Common.WebSocket
{
    public class GroupUsersDictionary<T>
    {
        private GroupUsersDictionary(){}
        private static readonly GroupUsersDictionary<T> _instance = new GroupUsersDictionary<T>();
        public static GroupUsersDictionary<T> GetInstance() => _instance;
        
        private readonly Dictionary<T, HashSet<Guid>> _connections =
            new Dictionary<T, HashSet<Guid>>();
        
        public void Add(T key, Guid userId)
        {
            lock (_connections)
            {
                if (!_connections.TryGetValue(key,out var userIds))
                {
                    userIds = new HashSet<Guid>();
                    _connections.Add(key, userIds);
                }
                lock (userIds)
                {
                    userIds.Add(userId);
                }
            }
        }
        
        public void Remove(T key, Guid userId)
        {
            lock (_connections)
            {
                if(!_connections.TryGetValue(key,out var userIds))
                { return; }
                lock (userIds)
                {
                    userIds.Remove(userId);
                    if (userIds.Count == 0)
                    { _connections.Remove(key); }
                }
            }
        }
        
        public IEnumerable<Guid> GetUserIds(T key)
        {
            lock (_connections)
            {
                if (_connections.TryGetValue(key,out var userIds))
                {
                    return userIds;
                }
            }

            return Enumerable.Empty<Guid>();
        }
    }
}