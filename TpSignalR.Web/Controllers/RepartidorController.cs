using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TpSignalR.Repositorio;
using TpSignalR.Web.Hubs;
using System.Linq;
using System.Threading.Tasks;

namespace TpSignalR.Web.Controllers
{
    public class RepartidorController : Controller
    {
        private readonly ServicioRepartoContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public RepartidorController(ServicioRepartoContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public IActionResult RepartidorLobby()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AceptarPedido([FromBody] AceptarPedidoModel model)
        {
            if (model == null)
                return BadRequest(new { error = "Model null" });

            var pedido = _context.Pedido.FirstOrDefault(p => p.PedidoId == model.PedidoId);
            if (pedido == null) return NotFound();

            // Solo se puede aceptar si está buscando repartidor
            if (!string.Equals(pedido.Estado, "buscando repartidor", System.StringComparison.OrdinalIgnoreCase))
                return Conflict(new { message = "Pedido ya no está disponible" });

            // Asignar repartidor y cambiar estado
            pedido.Estado = "en camino";
            pedido.RepartidorId = model.RepartidorId;
            _context.SaveChanges();

            // Notificar a todos los repartidores/clients so other repartidores remove the pedido from their lists
            await _hubContext.Clients.All.SendAsync("PedidoAceptado", pedido.PedidoId);

            // Notificar al usuario final (cliente) asociado al pedido using structured payload
            var userId = pedido.UsuarioFinalId;
            var clientePayload = new { role = "cliente", title = "Pedido", message = "Su pedido está en camino 🚚" };
            await _hubContext.Clients.Group($"user-{userId}").SendAsync("Notify", clientePayload);

            // Notificar al comercio correspondiente (ids fijos 1 y 4) with structured payload
            var comercioIds = new[] { 1, 4 };
            foreach (var cid in comercioIds)
            {
                var payload = new { role = "comercio", title = "Pedido", message = "El repartidor ya está enviando el pedido 🏍️." };
                _ = _hubContext.Clients.Group($"user-{cid}").SendAsync("Notify", payload)
                    .ContinueWith(t => { if (t.IsFaulted) System.Console.WriteLine($"❌ Notify error user-{cid}: {t.Exception?.GetBaseException().Message}"); }, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);
            }

            return Ok();
        }


        [HttpPost]
        public IActionResult RechazarPedido([FromBody] RechazarPedidoModel model)
        {
            if (model == null)
            {
                return BadRequest(new { error = "Model null" });
            }

            var pedido = _context.Pedido.FirstOrDefault(p => p.PedidoId == model.PedidoId);
            if (pedido == null) return NotFound();

            // No state change performed; keep behavior simple
            return Ok();
        }
    }

    public class AceptarPedidoModel
    {
        public int PedidoId { get; set; }
        public int RepartidorId { get; set; }
    }

    public class RechazarPedidoModel
    {
        public int PedidoId { get; set; }
        public int RepartidorId { get; set; }
    }
}
