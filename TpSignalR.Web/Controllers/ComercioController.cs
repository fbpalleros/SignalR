using Microsoft.AspNetCore.Mvc;
using PedidosYa.Web.Controllers;

namespace TpSignalR.Web.Controllers
{
    public class ComercioController : Controller
    {
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

    }
}
