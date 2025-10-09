using Microsoft.AspNetCore.Mvc;

namespace RegistroAduanero.Web.Controllers
{
    public class InicioController : Controller
    {
        public IActionResult Bienvenido()
        {
            return View();
        }
    }
}
