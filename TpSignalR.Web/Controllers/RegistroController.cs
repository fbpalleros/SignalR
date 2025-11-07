using Microsoft.AspNetCore.Http;
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
        public IActionResult LogInCookie(int userId)
        {
            var usuario = _registroLogica.ObtenerPorId(userId);
            // Guardamos nombre y id en session. No es necesario serializar a JSON para esto.
            HttpContext.Session.SetString("NombreUsuario", usuario.Nombre);
            HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
            return RedirectToAction("LogInCookie");
        }

        [HttpGet]
        public IActionResult LogInCookie()
        {
            return View();
        }

        public IActionResult LogIn()
        {

            var clientes = _registroLogica.obtenerTodos();
            ViewBag.Usuarios = clientes;

            return View();
        }
    }
}
