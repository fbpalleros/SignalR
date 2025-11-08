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

            // Crear el pedido (usuario 1, comercio 1 como ejemplo)
            var pedido = _pedidos.CrearPedido(1, 1, productoId);

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
                        Estado = pedido.Estado,
                        UsuarioFinalId = pedido.UsuarioFinalId,
                        ProductoId = producto.Id,
                        ProductoNombre = producto.Nombre,
                        ProductoCategoria = producto.Categoria,
                        ComercioId = pedido.ComercioId
                    };

                    // 🔔 Enviar al grupo del comercio correspondiente (si lo necesitás)
                    await _hubContext.Clients.Group($"user-{pedido.ComercioId}")
                        .SendAsync("NuevoPedido", pedidoInfo);

                    System.Console.WriteLine($"✅ Nuevo pedido enviado por SignalR al grupo user-{pedido.ComercioId}");

                    // Enviar notificaciones tailor-made a usuarios 1..6 según rol
                    var comercioIds = new[] { 1, 4 };
                    var clienteIds = new[] { 2, 6 };
                    var repartidorIds = new[] { 3, 5 };

                    // Build messages
                    string productName = producto.Nombre ?? "(producto)";

                    // Event: pedido creado
                    // Comercio: Tienes un nuevo pedido de "{producto}"
                    foreach (var cid in comercioIds)
                    {
                        var msg = $"Tienes un nuevo pedido de \"{productName}\"";
                        _ = _hubContext.Clients.Group($"user-{cid}").SendAsync("Notify", msg)
                            .ContinueWith(t => { if (t.IsFaulted) System.Console.WriteLine($"❌ Notify error user-{cid}: {t.Exception?.GetBaseException().Message}"); }, TaskContinuationOptions.OnlyOnFaulted);
                    }

                    // Repartidor: Tienes un pedido nuevo para entragar
                    foreach (var rid in repartidorIds)
                    {
                        var msg = $"Tienes un pedido nuevo para entragar";
                        _ = _hubContext.Clients.Group($"user-{rid}").SendAsync("Notify", msg)
                            .ContinueWith(t => { if (t.IsFaulted) System.Console.WriteLine($"❌ Notify error user-{rid}: {t.Exception?.GetBaseException().Message}"); }, TaskContinuationOptions.OnlyOnFaulted);
                    }

                    // Cliente: si quieres notificar a todos los clientes, enviá a los clienteIds
                    foreach (var cl in clienteIds)
                    {
                        var msg = $"Su pedido \"{productName}\" esta en preparacion"; // on creation -> preparacion
                        _ = _hubContext.Clients.Group($"user-{cl}").SendAsync("Notify", msg)
                            .ContinueWith(t => { if (t.IsFaulted) System.Console.WriteLine($"❌ Notify error user-{cl}: {t.Exception?.GetBaseException().Message}"); }, TaskContinuationOptions.OnlyOnFaulted);
                    }

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
