using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using TpSignalR.Logica;
using TpSignalR.Entidades;

namespace TpSignalR.Web.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMensajeLogica _mensajeLogica;

        public ChatHub(IMensajeLogica mensajeLogica)
        {
            _mensajeLogica = mensajeLogica;
        }

        public async Task EnviarMensaje(string usuario, string mensaje)
        {
            // Guardar mensaje usando la capa de lógica (que a su vez notificará vía IMensajeNotifier)
            var mensajeGuardado = _mensajeLogica.Guardar(mensaje, usuario);

            // Enviar ack al cliente que envió el mensaje (opcional)
            await Clients.Caller.SendAsync("MensajeGuardado", mensajeGuardado.Id);
        }
    }

    public class MensajeNotifier : IMensajeNotifier
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public MensajeNotifier(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyMensajeAsync(Mensaje mensaje)
        {
            await _hubContext.Clients.All.SendAsync("RecibirMensaje", mensaje.Usuario, mensaje.Texto, mensaje.Fecha.ToString("HH:mm:ss"));
        }
    }
}
