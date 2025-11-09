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

        /// <summary>
        /// Permite a un cliente unirse a un grupo específico para un pedido (por ejemplo "pedido-123").
        /// El frontend invoca connection.invoke('UnirseAPedido', pedidoId)
        /// </summary>
        /// <param name="pedidoId"></param>
        public async Task UnirseAPedido(int pedidoId)
        {
            try
            {
                var groupName = GetPedidoGroupName(pedidoId);
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                // opcional: log
                System.Console.WriteLine($"[NotificationHub] Conexión {Context.ConnectionId} unida al grupo {groupName}");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"[NotificationHub] Error UnirseAPedido: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Permite a un cliente salir del grupo pedido
        /// </summary>
        public async Task SalirDelPedido(int pedidoId)
        {
            try
            {
                var groupName = GetPedidoGroupName(pedidoId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
                System.Console.WriteLine($"[NotificationHub] Conexión {Context.ConnectionId} removida del grupo {groupName}");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"[NotificationHub] Error SalirDelPedido: {ex.Message}");
                throw;
            }
        }

        private string GetGroupName(int userId) => $"user-{userId}";
        private string GetPedidoGroupName(int pedidoId) => $"pedido-{pedidoId}";

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
