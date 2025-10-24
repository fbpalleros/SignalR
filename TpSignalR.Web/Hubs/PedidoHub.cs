using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace PedidosYa.Web.Hubs
{
    public class PedidoHub : Hub
    {
        public async Task UnirseAPedido(string pedidoId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, pedidoId);
        }

        public async Task EnviarUbicacion(string pedidoId, double lat, double lng)
        {
            await Clients.Group(pedidoId).SendAsync("RecibirUbicacion", lat, lng);
        }
    }
}
