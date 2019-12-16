using System;
using System.Collections.Generic;
using System.Linq;

namespace Squadio.Common.WebSocket
{
    public class GroupUsersDictionary<T>
    {
        private GroupUsersDictionary(){}
        private static readonly GroupUsersDictionary<T> Instance = new GroupUsersDictionary<T>();
        public static GroupUsersDictionary<T> GetInstance() => Instance;
        private readonly Dictionary<T, Dictionary<Guid, HashSet<string>>> _connections =
            new Dictionary<T, Dictionary<Guid, HashSet<string>>>();
        
        public void Add(T groupKey, Guid userId, string connectionId)
        {
            lock (_connections)
            {
                if (!_connections.TryGetValue(groupKey, out var users))
                {
                    users = new Dictionary<Guid, HashSet<string>>();
                    _connections.Add(groupKey, users);
                }
                lock (users)
                {
                    if (!users.TryGetValue(userId, out var conn))
                    {
                        conn = new HashSet<string>();
                        users.Add(userId, conn);
                    }
                    lock (conn)
                    {
                        conn.Add(connectionId);
                    }
                }
            }
        }

        public void Remove(string connectionId)
        {
            lock (_connections)
            {
                var groups = GetGroupsByConnection(connectionId);
                var userId = GetUserByConnection(connectionId);
                foreach (var groupKey in groups)
                {
                    var users = _connections[groupKey];
                    lock (users)
                    {
                        var user = users[userId];
                        lock (user)
                        {
                            user.Remove(connectionId);
                            if (user.Count == 0)
                                users.Remove(userId);
                        }
                    }

                    if (users.Count == 0)
                        _connections.Remove(groupKey);
                }
            }
        }

        public List<Guid> GetUsers(T groupKey)
        {
            var values = GetValues(groupKey);
            lock (values)
            {
                return values.Select(x => x.Key).ToList();
            }
        }
        
        public List<string> GetConnections(T groupKey, Guid userId)
        {
            var values = GetValues(groupKey);
            lock (values)
            {
                return values.TryGetValue(userId, out var conn) 
                    ? conn.ToList()
                    : new List<string>();
            }
        }

        public Guid GetUserByConnection(string connectionId)
        {
            lock (_connections)
            {
                var res = _connections.Select(group => 
                        group.Value.FirstOrDefault(user => user.Value.Contains(connectionId)))
                    .FirstOrDefault();

                return res.Key;
            }
        }
        
        private Dictionary<Guid, HashSet<string>> GetValues(T groupKey)
        {
            lock (_connections)
            {
                if (_connections.TryGetValue(groupKey, out var userIds))
                {
                    return userIds;
                }
            }
            return new Dictionary<Guid, HashSet<string>>();
        }

        private List<T> GetGroupsByConnection(string connectionId)
        {
            lock (_connections)
            {
                var res = _connections.Where(
                    group => group.Value.Contains(
                        group.Value.FirstOrDefault(user => user.Value.Contains(
                            connectionId))));
                
                return res.Select(x=>x.Key).ToList();
            }
        }
    }
}