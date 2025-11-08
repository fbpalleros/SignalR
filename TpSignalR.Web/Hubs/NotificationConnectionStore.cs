using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace TpSignalR.Web.Hubs
{
    public static class NotificationConnectionStore
    {
        private static readonly ConcurrentDictionary<string, int> ConnectionToUser = new();
        private static readonly ConcurrentDictionary<int, ConcurrentDictionary<string, byte>> UserToConnections = new();

        public static void Add(string connectionId, int userId)
        {
            ConnectionToUser[connectionId] = userId;
            var set = UserToConnections.GetOrAdd(userId, _ => new ConcurrentDictionary<string, byte>());
            set[connectionId] = 0;
        }

        public static void Remove(string connectionId)
        {
            if (ConnectionToUser.TryRemove(connectionId, out var userId))
            {
                if (UserToConnections.TryGetValue(userId, out var set))
                {
                    set.TryRemove(connectionId, out _);
                    if (set.IsEmpty)
                    {
                        UserToConnections.TryRemove(userId, out _);
                    }
                }
            }
        }

        public static bool TryGetUserForConnection(string connectionId, out int userId)
        {
            return ConnectionToUser.TryGetValue(connectionId, out userId);
        }

        public static IEnumerable<string> GetConnectionsForUser(int userId)
        {
            if (UserToConnections.TryGetValue(userId, out var set))
            {
                return set.Keys.ToList();
            }
            return Enumerable.Empty<string>();
        }
    }
}
