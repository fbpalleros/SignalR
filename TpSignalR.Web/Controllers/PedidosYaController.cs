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
            // Aquí podrías crear el pedido usando el productoId. Por ahora guardamos el id en TempData y redirigimos.
            TempData["ProductoSeleccionadoId"] = productoId;


            var pedido = _pedidos.CrearPedido(1, 1, productoId);

            // Enviar notificación por SignalR al usuario 1 (POC)
            try
            {
                var message = $"Pedido creado (ID producto: {productoId}, PedidoId: {pedido?.PedidoId})";
                await _hubContext.Clients.Group($"user-1").SendAsync("Notify", message);
            }
            catch (System.Exception ex)
            {
                // Logear o ignorar en POC
                System.Console.WriteLine($"Error enviando notificación SignalR: {ex.Message}");
            }

            return RedirectToAction("FinalizarCompra");
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
