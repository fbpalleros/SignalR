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
            Console.WriteLine("AceptarPedido endpoint invoked");
            Console.WriteLine($"Request Content-Type: {Request.ContentType}");
            Console.WriteLine($"Request Verification Token header: {Request.Headers["RequestVerificationToken"]}");

            if (model == null)
            {
                Console.WriteLine("AceptarPedido: model binding produced null model");
                return BadRequest(new { error = "Model null" });
            }

            Console.WriteLine($"AceptarPedido: PedidoId={model.PedidoId}, RepartidorId={model.RepartidorId}");

            var pedido = _context.Pedido.FirstOrDefault(p => p.PedidoId == model.PedidoId);
            if (pedido == null) return NotFound();

            // Only allow accepting if it's still looking for a repartidor
            if (pedido.Estado != "buscando repartidor")
            {
                return Conflict(new { message = "Pedido ya no está disponible" });
            }

            // Assign repartidor and change state
            pedido.Estado = "en camino";
            pedido.RepartidorId = model.RepartidorId;
            _context.SaveChanges();

            // Notify all clients so other repartidores remove the pedido from their lists
            await _hubContext.Clients.All.SendAsync("PedidoAceptado", pedido.PedidoId);

            // Notificar al usuario final si corresponde (usuarios 2 y 6)
            var userId = pedido.UsuarioFinalId;
            if (userId == 2 || userId == 6)
            {
                await _hubContext.Clients.Group($"user-{userId}").SendAsync("Notify", "Su pedido esta en camino");
            }

            return Ok();
        }

        [HttpPost]
        public IActionResult RechazarPedido([FromBody] RechazarPedidoModel model)
        {
            Console.WriteLine("RechazarPedido endpoint invoked");
            Console.WriteLine($"Request Content-Type: {Request.ContentType}");
            Console.WriteLine($"Request Verification Token header: {Request.Headers["RequestVerificationToken"]}");

            if (model == null)
            {
                Console.WriteLine("RechazarPedido: model binding produced null model");
                return BadRequest(new { error = "Model null" });
            }

            Console.WriteLine($"RechazarPedido: PedidoId={model.PedidoId}, RepartidorId={model.RepartidorId}");

            var pedido = _context.Pedido.FirstOrDefault(p => p.PedidoId == model.PedidoId);
            if (pedido == null) return NotFound();

            // Do nothing to the pedido (remains 'buscando repartidor'), but optionally log or notify
            // Notify the client who rejected (could be used for UX)
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
