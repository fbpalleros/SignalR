using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace TpSignalR.Web.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task Register(int userId)
        {
            // Si la conexión ya estaba asociada a otro usuario, la removemos del grupo anterior
            if (NotificationConnectionStore.TryGetUserForConnection(Context.ConnectionId, out var oldUserId) && oldUserId != userId)
            {
                var oldGroup = GetGroupName(oldUserId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, oldGroup);
            }

            // Mapeamos conexión -> usuario y agregamos al grupo
            NotificationConnectionStore.Add(Context.ConnectionId, userId);
            var groupName = GetGroupName(userId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task Unregister(int userId)
        {
            var groupName = GetGroupName(userId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            NotificationConnectionStore.Remove(Context.ConnectionId);
        }

        private string GetGroupName(int userId) => $"user-{userId}";

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (NotificationConnectionStore.TryGetUserForConnection(Context.ConnectionId, out var userId))
            {
                var groupName = GetGroupName(userId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
                NotificationConnectionStore.Remove(Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
