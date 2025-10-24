using Microsoft.AspNetCore.Mvc;
using TpSignalR.Entidades;
using TpSignalR.Logica;

namespace TpSignalR.Web.Controllers
{
    public class RegistroController : Controller
    {
        private readonly IUsuarioLogica _registroLogica;

        public RegistroController(IUsuarioLogica registroLogica)
        {
            _registroLogica = registroLogica;
        }

        [HttpPost]
        public IActionResult LogInCookie(string username, string userId)
        {
            Console.WriteLine($"Usuario: {username}");
            Usuario usuarioGenerado = new Usuario
            {
                Nombre = username
            };
            _registroLogica.registrarUsuario(usuarioGenerado);
            HttpContext.Session.SetString("NombreUsuario", username);
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
