using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TpSignalR.Repositorio;
using TpSignalR.Entidades;
using System.Linq;
using TpSignalR.Logica;
using TpSignalR.Web.Hubs;
using System.Threading.Tasks;

namespace TpSignalR.Web.Controllers
{
    public class PedidosYaController : Controller
    {
        private readonly IPedidosYaLogica _pedidos;
        private readonly IHubContext<NotificationHub> _hubContext;

        public PedidosYaController(IPedidosYaLogica pedidos, IHubContext<NotificationHub> hubContext)
        {
            _pedidos = pedidos;
            _hubContext = hubContext;
        }

        public IActionResult PedidosYaLobby()
        {
            return View();
        }
        public IActionResult ServicioSeleccionado()
        {
            // Obtener todos los productos y enviarlos como modelo a la vista
            var productos = _pedidos.ObtenerProductosPorComercio(1)
                .OrderBy(p => p.Id)
                .ToList();

            return View(productos);
        }

        public IActionResult FinalizarCompra()
        {
            return View();
        }

        public IActionResult VerPedidoEnTiempoReal()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> solicitarPedido(int productoId)
        {
            TempData["ProductoSeleccionadoId"] = productoId;

            // Use logged in user id from session if available
            var sessionUserId = HttpContext.Session.GetInt32("UsuarioId") ?? 1;

            // Crear el pedido usando el usuario en session
            var pedido = _pedidos.CrearPedido(sessionUserId, 1, productoId);

            try
            {
                var producto = _pedidos.ObtenerProductosPorComercio(1)
                    .FirstOrDefault(p => p.Id == productoId);

                if (pedido != null && producto != null)
                {
                    var pedidoInfo = new
                    {
                        PedidoId = pedido.PedidoId,
                        Total = producto.Precio,
                        UsuarioFinalId = pedido.UsuarioFinalId,
                        ProductoId = producto.Id,
                        ProductoNombre = producto.Nombre,
                        ProductoCategoria = producto.Categoria,
                        ComercioId = pedido.ComercioId
                    };

                    // Enviar al grupo del comercio correspondiente para actualizar su UI
                    await _hubContext.Clients.Group($"user-{pedido.ComercioId}")
                        .SendAsync("NuevoPedido", pedidoInfo);

                    System.Console.WriteLine($"✅ Nuevo pedido enviado por SignalR al grupo user-{pedido.ComercioId}");

                    // Enviar notificaciones estructuradas a comercios y repartidores, y al cliente que creó el pedido
                    var comercioIds = new[] { 1, 4 };
                    var repartidorIds = new[] { 3, 5 };

                    string productName = producto.Nombre ?? "(producto)";

                    // Comercio: Tienes un nuevo pedido de "{producto}"
                    foreach (var cid in comercioIds)
                    {
                        var payload = new { role = "comercio", title = "Nuevo pedido", message = $"Tienes un nuevo pedido de \"{productName}\"" };
                        _ = _hubContext.Clients.Group($"user-{cid}").SendAsync("Notify", payload)
                            .ContinueWith(t => { if (t.IsFaulted) System.Console.WriteLine($"❌ Notify error user-{cid}: {t.Exception?.GetBaseException().Message}"); }, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);
                    }

                    // Repartidor: Tienes un pedido nuevo para entregar
                    foreach (var rid in repartidorIds)
                    {
                        var payload = new { role = "repartidor", title = "Nuevo pedido", message = $"Tienes un pedido nuevo para entregar" };
                        _ = _hubContext.Clients.Group($"user-{rid}").SendAsync("Notify", payload)
                            .ContinueWith(t => { if (t.IsFaulted) System.Console.WriteLine($"❌ Notify error user-{rid}: {t.Exception?.GetBaseException().Message}"); }, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);
                    }

                    // Cliente: notify only the session user who created the pedido (structured payload)
                    var clientPayload = new { role = "cliente", title = "Pedido", message = $"Su pedido \"{productName}\" esta en preparacion" };
                    _ = _hubContext.Clients.Group($"user-{sessionUserId}").SendAsync("Notify", clientPayload)
                        .ContinueWith(t => { if (t.IsFaulted) System.Console.WriteLine($"❌ Notify error user-{sessionUserId}: {t.Exception?.GetBaseException().Message}"); }, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);

                    System.Console.WriteLine($"✅ Notificaciones de creación enviadas");
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"❌ Error enviando notificación SignalR: {ex.Message}");
            }

            // ✅ en vez de redirigir, devolvemos una respuesta 200 (sin refrescar la vista)
            return Ok(new { success = true, message = "Pedido enviado correctamente" });
        }



        public IActionResult PYLobby()
        {
            // Usar ruta física para evitar búsqueda por nombre de vista
            return View("~/Views/PedidosYa/PYLobby.cshtml");
        }




        public IActionResult Restaurantes()
        {
            var restaurantes = _pedidos.ObtenerComercios()
                .OrderBy(r => r.Nombre)
                .ToList();

            return View(restaurantes);
        }
    }
}

