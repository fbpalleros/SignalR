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

            // Si el nuevo estado es "buscando repartidor" notificamos al repartidor (usuario id 3)
            if (!string.IsNullOrEmpty(model.NuevoEstado) && model.NuevoEstado.ToLowerInvariant() == "buscando repartidor")
            {
                // Construir payload con información útil
                var producto = _context.Producto.FirstOrDefault(prod => prod.Id == pedido.ProductoId);
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

                // Enviar solo al usuario con id 3 (grupo "user-3") - fire-and-forget
                _hubContext.Clients.Group($"user-3").SendAsync("NuevoPedido", payload);
            }

            // Notificar a todos los usuarios de la app cuando cambia el estado
            string MapEstadoToMessage(string estado)
            {
                var e = (estado ?? string.Empty).ToLowerInvariant();
                if (e.Contains("pend")) return "Su pedido esta pendiente";
                if (e.Contains("camino") || e.Contains("en camino") || e.Contains("en_camino")) return "Su pedido esta en camino";
                if (e.Contains("entreg") || e.Contains("finaliz")) return "Su pedido ha sido entregado.";
                return null;
            }

            var notifyMessage = MapEstadoToMessage(model.NuevoEstado);
            if (!string.IsNullOrEmpty(notifyMessage))
            {
                // Enviar a todos (fire-and-forget)
                _hubContext.Clients.All.SendAsync("Notify", notifyMessage);
            }

            return Ok();
        }

    }
}
