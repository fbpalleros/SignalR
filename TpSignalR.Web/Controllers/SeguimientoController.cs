using Microsoft.AspNetCore.Mvc;

namespace PedidosYa.Web.Controllers
{
    public class SeguimientoController : Controller
    {
        public IActionResult Index(string pedidoId)
        {
            ViewBag.PedidoId = pedidoId ?? "pedido123"; // ID de ejemplo
            return View();
        }

        public IActionResult PruebaDelivery(string pedidoId)
        {
            ViewBag.PedidoId = pedidoId ?? "pedido123"; // ID de ejemplo
            return View();
        }
    }
}
