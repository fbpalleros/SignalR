using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using TpSignalR.Logica;

namespace PedidosYa.Web.Hubs
{
    public class PedidoHub : Hub
    {

        private IPedidosYaLogica _pedidosYaLogica;

        public PedidoHub(IPedidosYaLogica pedidosYaLogica)
        {
            _pedidosYaLogica = pedidosYaLogica;
        }

        public async Task EnviarPedido(int pedidoId, int comercioId)
        {
            await Clients.Group(comercioId.ToString()).SendAsync("RecibirPedido", pedidoId);
        }
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
