using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace TpSignalR.Web.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task Register(int userId)
        {
            var groupName = GetGroupName(userId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            Console.WriteLine($"NotificationHub: Connection {Context.ConnectionId} added to group {groupName}");
        }

        public async Task Unregister(int userId)
        {
            var groupName = GetGroupName(userId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            Console.WriteLine($"NotificationHub: Connection {Context.ConnectionId} removed from group {groupName}");
        }

        private string GetGroupName(int userId) => $"user-{userId}";

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            Console.WriteLine($"NotificationHub: Connection {Context.ConnectionId} disconnected. Exception: {exception?.Message}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
