using Microsoft.AspNetCore.Mvc;

namespace TpSignalR.Web.Controllers
{
    public class PedidosYaController : Controller
    {
        public IActionResult PedidosYa()
        {
            return View();
        }

        public IActionResult PedidosYaLobby()
        {
            return View();
        }
        public IActionResult ServicioSeleccionado()
        {
            return View();
        }

        public IActionResult FinalizarCompra()
        {
            return View();
        }

        public IActionResult VerPedidoEnTiempoReal()
        {
            return View();
        }
    }
}
