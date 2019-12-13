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
        
        private readonly Dictionary<Tuple<T, ConnectionGroup>, HashSet<Guid>> _connections =
            new Dictionary<Tuple<T, ConnectionGroup>, HashSet<Guid>>();
        
        public void Add(T key, ConnectionGroup group, Guid userId)
        {
            Add(new Tuple<T, ConnectionGroup>(key, group), userId);
        }
        
        public void Add(Tuple<T, ConnectionGroup> key, Guid userId)
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
        
        public void Remove(ConnectionGroup group, Guid userId)
        {
            lock (_connections)
            {
                var tuples = _connections
                    .Where(x => x.Value.Contains(userId)
                        && x.Key.Item2 == group)
                    .Select(x => x.Key)
                    .Distinct();
                foreach (var tuple in tuples)
                {
                    Remove(tuple, userId);
                }
            }
        }
        
        public void Remove(T key, ConnectionGroup group, Guid userId)
        {
            Remove(new Tuple<T, ConnectionGroup>(key, group), userId);
        }
        
        public void Remove(Tuple<T, ConnectionGroup> key, Guid userId)
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
        
        public IEnumerable<Guid> GetUserIds(T key, ConnectionGroup group)
        {
            return GetUserIds(new Tuple<T, ConnectionGroup>(key, group));
        }
        
        public IEnumerable<Guid> GetUserIds(Tuple<T, ConnectionGroup> key)
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