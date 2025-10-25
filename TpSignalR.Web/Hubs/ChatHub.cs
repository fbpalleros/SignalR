using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using TpSignalR.Logica;

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
            // Guardar en la base de datos
            var mensajeGuardado = _mensajeLogica.Guardar(mensaje, usuario);

            // Enviar a todos los clientes conectados
            await Clients.All.SendAsync("RecibirMensaje", 
                mensajeGuardado.Usuario, 
                mensajeGuardado.Texto, 
                mensajeGuardado.Fecha.ToString("HH:mm:ss"));
        }
    }
}
