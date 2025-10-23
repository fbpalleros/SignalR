using Microsoft.AspNetCore.Mvc;
using TpSignalR.Entidades;
using TpSignalR.Logica;

namespace TpSignalR.Web.Controllers
{
    public class RegistroController : Controller
    {
        private readonly IRegistroLogica _registroLogica;

        public RegistroController(IRegistroLogica registroLogica)
        {
            _registroLogica = registroLogica;
        }

        [HttpPost]
        public IActionResult LogInCookie(string username, string userId)
        {
            Console.WriteLine($"Usuario: {username}, ID: {userId}");
            TempData["UserId"] = userId;
            Usuario usuarioGenerado = new Usuario
            {
                Nombre = username,
                IdUsuario = int.Parse(userId)
            };
            _registroLogica.registrarUsuario(usuarioGenerado);
            return RedirectToAction("LogInCookie");
        }

        [HttpGet]
        public IActionResult LogInCookie()
        {
            return View();
        }

        public IActionResult LogIn()
        {
            return View();
        }
    }
}
