using Microsoft.AspNetCore.Mvc;

namespace TpSignalR.Web.Controllers
{
    public class InicioController : Controller
    {
        public IActionResult Home()
        {
            return View();
        }

        public IActionResult LogIn()
        {
            return View();
        }
    }
}
