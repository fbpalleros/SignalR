using Microsoft.AspNetCore.Mvc;
using RegistroAduanero.Entidades;
using RegistroAduanero.Logica;

namespace RegistroAduanero.Web.Controllers
{
    public class BarcosController : Controller
    {


        private readonly IBarcosLogica barcosLogica;

        public BarcosController(IBarcosLogica barcosLogica)
        {
            this.barcosLogica = barcosLogica;
        }

        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registrar(Barco barco)
        {


            if (ModelState.IsValid)
            {
                barcosLogica.registrarBarco(barco);
                TempData["Mensaje"] = $"El barco {barco.Nombre} fue registrado con un impuesto de {barco.Impuesto:C2}";
                return RedirectToAction("Resultados");
            }
            return View(barco);
        }

        public IActionResult Resultados()
        {
            var barcos = barcosLogica.obtenerTodos();
            return View(barcos);
        }
    }
}
