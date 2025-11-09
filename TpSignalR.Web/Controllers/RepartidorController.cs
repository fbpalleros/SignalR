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

        // ✅ Repartidor acepta un pedido
        [HttpPost]
        public async Task<IActionResult> AceptarPedido([FromBody] AceptarPedidoModel model)
        {
            Console.WriteLine("AceptarPedido endpoint invoked");

            if (model == null)
                return BadRequest(new { error = "Model null" });

            var pedido = _context.Pedido.FirstOrDefault(p => p.PedidoId == model.PedidoId);
            if (pedido == null) return NotFound();

            // Solo puede aceptarse si está buscando repartidor
            if (pedido.Estado != "buscando repartidor")
                return Conflict(new { message = "Pedido ya no está disponible" });

            // Asignar repartidor y actualizar estado
            pedido.Estado = "en camino";
            pedido.RepartidorId = model.RepartidorId;
            _context.SaveChanges();

            // 🔹 1. Notificar a todos que este pedido fue tomado
            await _hubContext.Clients.All.SendAsync("PedidoAceptado", pedido.PedidoId);

            // 🔹 2. Notificar evento que inicia el mapa (PedidoEnCamino)
            await _hubContext.Clients.All.SendAsync("PedidoEnCamino", new
            {
                PedidoId = pedido.PedidoId,
                ComercioLat = -34.615803, // reemplazá por pedido.Comercio.Latitud
                ComercioLng = -58.433297, // reemplazá por pedido.Comercio.Longitud
                UsuarioLat = -34.653480,  // reemplazá por pedido.UsuarioFinal.Latitud
                UsuarioLng = -58.668710,  // reemplazá por pedido.UsuarioFinal.Longitud
                Estado = "en camino"
            });

            // 🔹 3. Confirmar al cliente JS que todo salió bien
            return Json(new { success = true, pedidoId = pedido.PedidoId });
        }

        [HttpPost]
        public IActionResult RechazarPedido([FromBody] RechazarPedidoModel model)
        {
            if (model == null)
                return BadRequest(new { error = "Model null" });

            var pedido = _context.Pedido.FirstOrDefault(p => p.PedidoId == model.PedidoId);
            if (pedido == null) return NotFound();

            // Mantiene el estado "buscando repartidor"
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
