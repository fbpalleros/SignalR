using Microsoft.AspNetCore.Mvc;
using PedidosYa.Web.Controllers;
using TpSignalR.Repositorio;
using TpSignalR.Web.Models;
using System.Linq;
using System.Collections.Generic;
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
            // Redirige a la acción "PruebaIngresoDeDatosDelivery" del controlador "SeguimientoController"
            // Nota: el nombre del controlador se pasa sin el sufijo "Controller"
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ActualizarEstadoPedido([FromBody] EstadoUpdateModel model)
        {
            if (model == null) return BadRequest();

            var pedido = _context.Pedido.FirstOrDefault(p => p.PedidoId == model.PedidoId);
            if (pedido == null) return NotFound();

            pedido.Estado = model.NuevoEstado;
            _context.SaveChanges();

            // Obtener nombre de producto para mensajes
            var producto = _context.Producto.FirstOrDefault(prod => prod.Id == pedido.ProductoId);
            var productName = producto?.Nombre ?? "(producto)";

            // Definir grupos por rol
            var comercioIds = new[] { 1, 4 };
            var clienteIds = new[] { 2, 6 };
            var repartidorIds = new[] { 3, 5 };

            var estadoLower = (model.NuevoEstado ?? string.Empty).ToLowerInvariant();

            // Cuando el pedido entra en preparación
            if (estadoLower.Contains("prep") || estadoLower.Contains("prepar"))
            {
                // Comercio: Se esta preparando el pedido
                foreach (var cid in comercioIds)
                {
                    var msg = $"Se esta preparando el pedido \"{productName}\"";
                    _ = _hubContext.Clients.Group($"user-{cid}").SendAsync("Notify", msg)
                        .ContinueWith(t => { if (t.IsFaulted) System.Console.WriteLine($"❌ Notify error user-{cid}: {t.Exception?.GetBaseException().Message}"); }, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);
                }

                // Cliente: Su pedido "X" esta en preparacion
                foreach (var cl in clienteIds)
                {
                    var msg = $"Su pedido \"{productName}\" esta en preparacion";
                    _ = _hubContext.Clients.Group($"user-{cl}").SendAsync("Notify", msg)
                        .ContinueWith(t => { if (t.IsFaulted) System.Console.WriteLine($"❌ Notify error user-{cl}: {t.Exception?.GetBaseException().Message}"); }, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);
                }
            }

            // Cuando el pedido pasa a 'en camino'
            if (estadoLower.Contains("camino") || estadoLower.Contains("en camino") || estadoLower.Contains("en_camino"))
            {
                // Cliente: Su pedido "X" esta en camino
                foreach (var cl in clienteIds)
                {
                    var msg = $"Su pedido \"{productName}\" esta en camino";
                    _ = _hubContext.Clients.Group($"user-{cl}").SendAsync("Notify", msg)
                        .ContinueWith(t => { if (t.IsFaulted) System.Console.WriteLine($"❌ Notify error user-{cl}: {t.Exception?.GetBaseException().Message}"); }, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);
                }

                // Repartidor: Estas enviando el pedido "X"
                foreach (var rid in repartidorIds)
                {
                    var msg = $"Estas enviando el pedido \"{productName}\"";
                    _ = _hubContext.Clients.Group($"user-{rid}").SendAsync("Notify", msg)
                        .ContinueWith(t => { if (t.IsFaulted) System.Console.WriteLine($"❌ Notify error user-{rid}: {t.Exception?.GetBaseException().Message}"); }, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);
                }

            }

            // Cuando el pedido se finaliza / entregado
            if (estadoLower.Contains("entreg") || estadoLower.Contains("finaliz"))
            {
                // Cliente: Su pedido ha sido entregado
                foreach (var cl in clienteIds)
                {
                    var msg = $"Su pedido \"{productName}\" ha sido entregado.";
                    _ = _hubContext.Clients.Group($"user-{cl}").SendAsync("Notify", msg)
                        .ContinueWith(t => { if (t.IsFaulted) System.Console.WriteLine($"❌ Notify error user-{cl}: {t.Exception?.GetBaseException().Message}"); }, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);
                }

                // Repartidor: Entregaste el pedido "X"
                foreach (var rid in repartidorIds)
                {
                    var msg = $"Entregaste el pedido \"{productName}\"";
                    _ = _hubContext.Clients.Group($"user-{rid}").SendAsync("Notify", msg)
                        .ContinueWith(t => { if (t.IsFaulted) System.Console.WriteLine($"❌ Notify error user-{rid}: {t.Exception?.GetBaseException().Message}"); }, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);
                }

                // Comercio: El pedido de tu tienda ha sido entregado correctamente
                foreach (var cid in comercioIds)
                {
                    var msg = $"El pedido de tu tienda ha sido entregado correctamente";
                    _ = _hubContext.Clients.Group($"user-{cid}").SendAsync("Notify", msg)
                        .ContinueWith(t => { if (t.IsFaulted) System.Console.WriteLine($"❌ Notify error user-{cid}: {t.Exception?.GetBaseException().Message}"); }, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);
                }
            }

            return Ok();
        }

    }
}
