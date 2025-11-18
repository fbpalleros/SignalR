using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using TpSignalR.Logica;

namespace TpSignalR.Web.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly IPedidosYaLogica _pedidosLogica;

        public NotificationHub(IPedidosYaLogica pedidosLogica)
        {
            _pedidosLogica = pedidosLogica;
        }

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

            Console.WriteLine($"NotificationHub: Connection {Context.ConnectionId} added to group {groupName}");

            // After registering, send current 'buscando repartidor' pedidos for this comercio (if any)
            try
            {
                // Obtener pedidos del comercio (userId may represent comercio id in this app)
                var pedidos = _pedidosLogica.ObtenerPedidosPorComercio(userId);
                if (pedidos != null)
                {
                    foreach (var p in pedidos)
                    {
                        // Only send pedidos that are in 'buscando repartidor' state
                        if (string.Equals(p.Estado, "buscando repartidor", System.StringComparison.OrdinalIgnoreCase))
                        {
                            var payload = new
                            {
                                PedidoId = p.PedidoId,
                                Total = p.Total,
                                Estado = p.Estado,
                                UsuarioFinalId = p.UsuarioFinalId,
                                ProductoId = p.ProductoId,
                                ProductoNombre = p.ProductoNombre,
                                ProductoCategoria = p.ProductoCategoria,
                                ComercioId = p.ComercioId
                            };

                            await Clients.Caller.SendAsync("NuevoPedido", payload);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"NotificationHub: error while sending existing pedidos to caller: {ex.Message}");
            }
        }

        public async Task Unregister(int userId)
        {
            var groupName = GetGroupName(userId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            Console.WriteLine($"NotificationHub: Connection {Context.ConnectionId} removed from group {groupName}");
            NotificationConnectionStore.Remove(Context.ConnectionId);
        }

        private string GetGroupName(int userId) => $"user-{userId}";

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            if (NotificationConnectionStore.TryGetUserForConnection(Context.ConnectionId, out var userId))
            {
                var groupName = GetGroupName(userId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
                NotificationConnectionStore.Remove(Context.ConnectionId);
            }

            Console.WriteLine($"NotificationHub: Connection {Context.ConnectionId} disconnected. Exception: {exception?.Message}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
