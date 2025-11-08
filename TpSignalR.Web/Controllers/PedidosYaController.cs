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
                        UsuarioFinalId = pedido.UsuarioFinalId,
                        ProductoId = producto.Id,
                        ProductoNombre = producto.Nombre,
                        ProductoCategoria = producto.Categoria,
                        ComercioId = pedido.ComercioId
                    };

                    // 🔔 Enviar al grupo del comercio correspondiente
                    await _hubContext.Clients.Group($"user-{pedido.ComercioId}")
                        .SendAsync("NuevoPedido", pedidoInfo);

                    System.Console.WriteLine($"✅ Nuevo pedido enviado por SignalR al grupo user-{pedido.ComercioId}");
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
