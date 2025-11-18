using Microsoft.AspNetCore.Mvc;
using TpSignalR.Web.Models;
using TpSignalR.Logica;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TpSignalR.Web.Hubs;
using System.Collections.Generic;

namespace TpSignalR.Web.Controllers
{
    public class ComercioController : Controller
    {
        private readonly IPedidosYaLogica _pedidosYaLogica;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ComercioController(IPedidosYaLogica pedidosYaLogica, IHubContext<NotificationHub> hubContext)
        {
            _pedidosYaLogica = pedidosYaLogica;
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
            var pedidosDto = _pedidosYaLogica.ObtenerPedidosPorComercio(comercioId);
            var pedidos = pedidosDto.Select(p => new ComercioPedidoViewModel
            {
                PedidoId = p.PedidoId,
                Total = p.Total,
                Estado = p.Estado,
                UsuarioFinalId = p.UsuarioFinalId,
                ProductoId = p.ProductoId,
                ProductoNombre = p.ProductoNombre,
                ProductoCategoria = p.ProductoCategoria
            }).ToList();

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

            // Update via service and get DTO with refreshed data
            var dto = await _pedidosYaLogica.ActualizarEstadoPedidoAsync(model.PedidoId, model.NuevoEstado);

            var estadoLower = (dto?.Estado ?? string.Empty).ToLowerInvariant();
            var productName = dto?.ProductoNombre ?? "(producto)";

            // Cliente real del pedido
            var clienteUserId = dto?.UsuarioFinalId ?? 0;
            var demoClienteIds = new[] { 2, 6 };

            // --- ESTADO: PREPARACIÓN ---
            if (estadoLower.Contains("prep") || estadoLower.Contains("prepar"))
            {
                var clientMsg = $"Su pedido \"{productName}\" está en preparación";

                // Cliente real
                if (clienteUserId > 0) SendNotify(clienteUserId, "cliente", clientMsg, "Pedido");

                // Clientes demo
                foreach (var cid in demoClienteIds)
                {
                    if (cid != clienteUserId)
                        SendNotify(cid, "cliente", clientMsg, "Pedido");
                }

                // Comercios (example ids)
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

                if (clienteUserId > 0) SendNotify(clienteUserId, "cliente", clientMsg, "Pedido");
                foreach (var cid in demoClienteIds) if (cid != clienteUserId) SendNotify(cid, "cliente", clientMsg, "Pedido");

                var repartidorIds = new[] { 3, 5 };
                foreach (var rid in repartidorIds)
                {
                    var msg = $"Estás enviando el pedido \"{productName}\"";
                    SendNotify(rid, "repartidor", msg, "Pedido");
                }

                var comercioIds = new[] { 1, 4 };
                foreach (var cid in comercioIds)
                {
                    var msg = $"El repartidor ya está enviando el pedido.";
                    SendNotify(cid, "comercio", msg, "Pedido");
                }

                // Also send PedidoEnCamino payload to user and pedido group
                var payloadToUser = new
                {
                    PedidoId = dto.PedidoId,
                    Estado = dto.Estado,
                    Total = dto.Total,
                    ProductoId = dto.ProductoId,
                    ProductoNombre = dto.ProductoNombre,
                    ProductoCategoria = dto.ProductoCategoria,
                    ComercioId = dto.ComercioId,
                    ComercioNombre = dto.ComercioNombre,
                    ComercioLat = dto.ComercioLat,
                    ComercioLng = dto.ComercioLng,
                    UsuarioFinalId = dto.UsuarioFinalId
                };

                await _hubContext.Clients.Group($"user-{dto.UsuarioFinalId}").SendAsync("PedidoEnCamino", payloadToUser);
                await _hubContext.Clients.Group($"pedido-{dto.PedidoId}").SendAsync("PedidoEnCamino", payloadToUser);
            }

            // --- ESTADO: ENTREGADO / FINALIZADO ---
            if (estadoLower.Contains("entreg") || estadoLower.Contains("finaliz"))
            {
                var clientMsg = $"Su pedido \"{productName}\" ha sido entregado.";
                if (clienteUserId > 0) SendNotify(clienteUserId, "cliente", clientMsg, "Pedido");
                foreach (var cid in demoClienteIds) if (cid != clienteUserId) SendNotify(cid, "cliente", clientMsg, "Pedido");

                var repartidorIds = new[] { 3, 5 };
                foreach (var rid in repartidorIds) SendNotify(rid, "repartidor", $"Entregaste el pedido \"{productName}\"", "Pedido");
                var comercioIds = new[] { 1, 4 };
                foreach (var cid in comercioIds) SendNotify(cid, "comercio", $"El pedido de tu tienda ha sido entregado correctamente.", "Pedido");
            }

            // --- NUEVO ESTADO: BUSCANDO REPARTIDOR ---
            if (string.Equals(dto.Estado, "buscando repartidor", System.StringComparison.OrdinalIgnoreCase))
            {
                var payload = new ComercioPedidoViewModel
                {
                    PedidoId = dto.PedidoId,
                    Total = dto.Total,
                    Estado = dto.Estado,
                    UsuarioFinalId = dto.UsuarioFinalId,
                    ProductoId = dto.ProductoId,
                    ProductoNombre = dto.ProductoNombre,
                    ProductoCategoria = dto.ProductoCategoria
                };

                await _hubContext.Clients.Group($"user-3").SendAsync("NuevoPedido", payload);

                // Notificar a repartidores (mismo comportamiento que el snippet movido desde PedidosYaController)
                var repartidorIds = new[] { 3, 5 };
                foreach (var rid in repartidorIds)
                {
                    // Enviar notificación estructurada al repartidor
                    SendNotify(rid, "repartidor", "Tienes un pedido nuevo para entregar", "Nuevo pedido");
                }
            }

            // Return updated DTO to caller so client can refresh UI
            return Ok(dto);
        }

        [HttpGet]
        public IActionResult GetPedidosJson(int comercioId = 1)
        {
            var pedidosDto = _pedidosYaLogica.ObtenerPedidosPorComercio(comercioId);
            var pedidos = pedidosDto.Select(p => new ComercioPedidoViewModel
            {
                PedidoId = p.PedidoId,
                Total = p.Total,
                Estado = p.Estado,
                UsuarioFinalId = p.UsuarioFinalId,
                ProductoId = p.ProductoId,
                ProductoNombre = p.ProductoNombre,
                ProductoCategoria = p.ProductoCategoria
            }).ToList();

            return Json(pedidos);
        }
    }
}
