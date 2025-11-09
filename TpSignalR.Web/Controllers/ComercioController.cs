using Microsoft.AspNetCore.Mvc;
using TpSignalR.Repositorio;
using TpSignalR.Web.Models;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using TpSignalR.Web.Hubs;

namespace TpSignalR.Web.Controllers
{
    public class ComercioController : Controller
    {
        private readonly ServicioRepartoContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ComercioController(ServicioRepartoContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public IActionResult Inicio()
        {
            return View();
        }

        public IActionResult DetallePedido()
        {
            return RedirectToAction("PruebaIngresoDeDatosDelivery", "Seguimiento");
        }

        public IActionResult ComercioLobby()
        {
            int comercioId = 1;
            var pedidos = _context.Pedido
                .Where(p => p.ComercioId == comercioId)
                .Select(p => new ComercioPedidoViewModel
                {
                    PedidoId = p.PedidoId,
                    Total = p.Total,
                    Estado = p.Estado,
                    UsuarioFinalId = p.UsuarioFinalId,
                    ProductoId = p.ProductoId,
                    ProductoNombre = _context.Producto.Where(prod => prod.Id == p.ProductoId).Select(prod => prod.Nombre).FirstOrDefault(),
                    ProductoCategoria = _context.Producto.Where(prod => prod.Id == p.ProductoId).Select(prod => prod.Categoria).FirstOrDefault()
                })
                .ToList();

            ViewBag.Pedidos = pedidos;
            return View();
        }

        // Helper to send structured notify payload to a group
        private void SendNotify(int userId, string role, string message, string title = "Pedido")
        {
            var payload = new { role = role, title = title, message = message };
            _ = _hubContext.Clients.Group($"user-{userId}").SendAsync("Notify", payload)
                .ContinueWith(t => { if (t.IsFaulted) System.Console.WriteLine($"❌ Notify error user-{userId}: {t.Exception?.GetBaseException().Message}"); }, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);
        }

        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ActualizarEstadoPedido([FromBody] EstadoUpdateModel model)
{
    if (model == null) return BadRequest();

    var pedido = _context.Pedido.FirstOrDefault(p => p.PedidoId == model.PedidoId);
    if (pedido == null) return NotFound();

    pedido.Estado = model.NuevoEstado;
    await _context.SaveChangesAsync();

    // Obtener nombre de producto y comercio
    var producto = _context.Producto.FirstOrDefault(prod => prod.Id == pedido.ProductoId);
    var comercio = _context.Comercio.FirstOrDefault(c => c.Id == pedido.ComercioId);
    var productName = producto?.Nombre ?? "(producto)";
    var estadoLower = (model.NuevoEstado ?? string.Empty).ToLowerInvariant();

    // Cliente real del pedido
    var clienteUserId = pedido.UsuarioFinalId;

    // IDs de prueba (clientes demo)
    var demoClienteIds = new[] { 2, 6 };

    // --- ESTADO: PREPARACIÓN ---
    if (estadoLower.Contains("prep") || estadoLower.Contains("prepar"))
    {
        var clientMsg = $"Su pedido \"{productName}\" está en preparación";

        // Cliente real
        SendNotify(clienteUserId, "cliente", clientMsg, "Pedido");

        // Clientes demo
        foreach (var cid in demoClienteIds)
        {
            if (cid != clienteUserId)
                SendNotify(cid, "cliente", clientMsg, "Pedido");
        }

        // Comercios
        var comercioIds = new[] { 1, 4 };
        foreach (var cid in comercioIds)
        {
            var msg = $"Se está preparando el pedido \"{productName}\"";
            SendNotify(cid, "comercio", msg, "Pedido");
        }

        // Repartidores
        var repartidorIds = new[] { 3, 5 };
        foreach (var rid in repartidorIds)
        {
            var msg = $"Tienes un pedido nuevo para entregar";
            SendNotify(rid, "repartidor", msg, "Pedido");
        }
    }

    // --- ESTADO: EN CAMINO ---
    if (estadoLower.Contains("camino") || estadoLower.Contains("en camino") || estadoLower.Contains("en_camino"))
    {
        var clientMsg = $"Su pedido \"{productName}\" está en camino";

        // Cliente real
        SendNotify(clienteUserId, "cliente", clientMsg, "Pedido");

        // Clientes demo
        foreach (var cid in demoClienteIds)
        {
            if (cid != clienteUserId)
                SendNotify(cid, "cliente", clientMsg, "Pedido");
        }

        // Repartidores
        var repartidorIds = new[] { 3, 5 };
        foreach (var rid in repartidorIds)
        {
            var msg = $"Estás enviando el pedido \"{productName}\"";
            SendNotify(rid, "repartidor", msg, "Pedido");
        }

        // Comercios
        var comercioIds = new[] { 1, 4 };
        foreach (var cid in comercioIds)
        {
            var msg = $"El repartidor ya está enviando el pedido.";
            SendNotify(cid, "comercio", msg, "Pedido");
        }

        // 🔔 También notificar al usuario con payload completo (nueva lógica)
        var payloadToUser = new
        {
            PedidoId = pedido.PedidoId,
            Estado = pedido.Estado,
            Total = pedido.Total,
            ProductoId = pedido.ProductoId,
            ProductoNombre = producto?.Nombre,
            ProductoCategoria = producto?.Categoria,
            ComercioId = pedido.ComercioId,
            ComercioNombre = comercio?.Nombre,
            ComercioLat = comercio?.Latitud,
            ComercioLng = comercio?.Longitud,
            UsuarioFinalId = pedido.UsuarioFinalId
        };

        await _hubContext.Clients.Group($"user-{pedido.UsuarioFinalId}")
            .SendAsync("PedidoEnCamino", payloadToUser);

        await _hubContext.Clients.Group($"pedido-{pedido.PedidoId}")
            .SendAsync("PedidoEnCamino", payloadToUser);
    }

    // --- ESTADO: ENTREGADO / FINALIZADO ---
    if (estadoLower.Contains("entreg") || estadoLower.Contains("finaliz"))
    {
        var clientMsg = $"Su pedido \"{productName}\" ha sido entregado.";

        // Cliente real
        SendNotify(clienteUserId, "cliente", clientMsg, "Pedido");

        // Clientes demo
        foreach (var cid in demoClienteIds)
        {
            if (cid != clienteUserId)
                SendNotify(cid, "cliente", clientMsg, "Pedido");
        }

        // Repartidores
        var repartidorIds = new[] { 3, 5 };
        foreach (var rid in repartidorIds)
        {
            var msg = $"Entregaste el pedido \"{productName}\"";
            SendNotify(rid, "repartidor", msg, "Pedido");
        }

        // Comercios
        var comercioIds = new[] { 1, 4 };
        foreach (var cid in comercioIds)
        {
            var msg = $"El pedido de tu tienda ha sido entregado correctamente.";
            SendNotify(cid, "comercio", msg, "Pedido");
        }
    }

    // --- NUEVO ESTADO: BUSCANDO REPARTIDOR ---
    if (string.Equals(model.NuevoEstado, "buscando repartidor", StringComparison.OrdinalIgnoreCase))
    {
        var payload = new ComercioPedidoViewModel
        {
            PedidoId = pedido.PedidoId,
            Total = pedido.Total,
            Estado = pedido.Estado,
            UsuarioFinalId = pedido.UsuarioFinalId,
            ProductoId = pedido.ProductoId,
            ProductoNombre = producto?.Nombre,
            ProductoCategoria = producto?.Categoria
        };

        // Enviar solo al repartidor (usuario id 3)
        await _hubContext.Clients.Group($"user-3").SendAsync("NuevoPedido", payload);
    }

    return Ok();
}



    }
}
