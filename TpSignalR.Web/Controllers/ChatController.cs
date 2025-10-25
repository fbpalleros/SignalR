using Microsoft.AspNetCore.Mvc;
using TpSignalR.Logica;

namespace TpSignalR.Web.Controllers
{
    public class ChatController : Controller
    {
        private readonly IMensajeLogica _mensajeLogica;

        public ChatController(IMensajeLogica mensajeLogica)
        {
            _mensajeLogica = mensajeLogica;
        }

        public IActionResult Index()
        {
            var mensajes = _mensajeLogica.ObtenerTodos();
            return View(mensajes);
        }
    }
}
