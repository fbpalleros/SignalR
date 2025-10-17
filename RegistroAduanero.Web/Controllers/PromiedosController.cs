using Microsoft.AspNetCore.Mvc;
using RegistroAduanero.Entidades;
using RegistroAduanero.Logica;

namespace RegistroAduanero.Web.Controllers
{
    public class PromiedosController : Controller
    {


        private readonly IBarcosLogica barcosLogica;

        public PromiedosController(IBarcosLogica barcosLogica)
        {
            this.barcosLogica = barcosLogica;
        }

        public IActionResult Promiedos()
        {
            return View();
        }
        public IActionResult PromiedosLobby()
        {
            return View();
        }

        public IActionResult DetallePartido()
        {
            return View();
        }

        public IActionResult Categoria()
        {
            return View();
        }

        public IActionResult DetalleEquipo()
        {
            return View();
        }







        [HttpPost]
        public IActionResult Promiedos(Barco barco)
        {


            if (ModelState.IsValid)
            {
                barcosLogica.registrarBarco(barco);
                TempData["Mensaje"] = $"El barco {barco.Nombre} fue registrado con un impuesto de {barco.Impuesto:C2}";
                return RedirectToAction("PedidosYa");
            }
            return View(barco);
        }

        public IActionResult PedidosYa()
        {
            var barcos = barcosLogica.obtenerTodos();
            return View(barcos);
        }
    }
}
