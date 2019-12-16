using System;
using System.Collections.Generic;

namespace Squadio.Common.WebSocket
{
    public class GroupUsersDictionary<T>
    {
        private GroupUsersDictionary(){}
        private static readonly GroupUsersDictionary<T> Instance = new GroupUsersDictionary<T>();
        public static GroupUsersDictionary<T> GetInstance() => Instance;
        private readonly Dictionary<T, HashSet<string>> _connections =
            new Dictionary<T, HashSet<string>>();
        
        public void Add(T key, Guid userId, string connectionId)
        {
        }
        
        public void Remove(string connectionId)
        {
        }
    }
}