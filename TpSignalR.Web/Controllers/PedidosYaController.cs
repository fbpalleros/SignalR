using Microsoft.AspNetCore.Mvc;
using TpSignalR.Repositorio;
using TpSignalR.Entidades;
using System.Linq;
using TpSignalR.Logica;

namespace TpSignalR.Web.Controllers
{
    public class PedidosYaController : Controller
    {
        private readonly IPedidosYaLogica _pedidos;

        public PedidosYaController(IPedidosYaLogica pedidos)
        {
            _pedidos = pedidos;
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
        public IActionResult solicitarPedido(int productoId)
        {
            // Aquí podrías crear el pedido usando el productoId. Por ahora guardamos el id en TempData y redirigimos.
            TempData["ProductoSeleccionadoId"] = productoId;
            _pedidos.CrearPedido(1, 1, 1);
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
