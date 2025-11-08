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
        public async Task<IActionResult> ActualizarEstadoPedido([FromBody] EstadoUpdateModel model)
        {
            if (model == null) return BadRequest();

            var pedido = _context.Pedido.FirstOrDefault(p => p.PedidoId == model.PedidoId);
            if (pedido == null) return NotFound();

            pedido.Estado = model.NuevoEstado;
            await _context.SaveChangesAsync();

            // Si el nuevo estado es "buscando repartidor" notificamos al repartidor (usuario id 3)
            if (string.Equals(model.NuevoEstado, "buscando repartidor", StringComparison.OrdinalIgnoreCase))
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

                // Enviar solo al usuario con id 3 (grupo "user-3")
                await _hubContext.Clients.Group($"user-3").SendAsync("NuevoPedido", payload);
            }

            // Si el nuevo estado es "en camino" notificamos al usuario que realizó el pedido
            if (string.Equals(model.NuevoEstado, "en camino", StringComparison.OrdinalIgnoreCase))
            {
                var producto = _context.Producto.FirstOrDefault(prod => prod.Id == pedido.ProductoId);
                var comercio = _context.Comercio.FirstOrDefault(c => c.Id == pedido.ComercioId);

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

                // Enviar al grupo del usuario (user-{id}) y al grupo del pedido (pedido-{id})
                await _hubContext.Clients.Group($"user-{pedido.UsuarioFinalId}")
                    .SendAsync("PedidoEnCamino", payloadToUser);

                await _hubContext.Clients.Group($"pedido-{pedido.PedidoId}")
                    .SendAsync("PedidoEnCamino", payloadToUser);
            }

            return Ok();
        }

    }
}
