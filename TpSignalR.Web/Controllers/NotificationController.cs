using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using TpSignalR.Web.Hubs;

namespace TpSignalR.Web.Controllers
{
    public class NotificationController : Controller
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        // GET /Notification/SendToUser?userId=1&message=hola
        [HttpGet]
        public async Task<IActionResult> SendToUser(int userId, string message = null)
        {
            var payload = string.IsNullOrEmpty(message) ? $"Mensaje de prueba para usuario {userId}" : message;
            await _hubContext.Clients.Group($"user-{userId}").SendAsync("Notify", payload);
            return Content($"Enviado a user-{userId}: {payload}");
        }
    }
}
