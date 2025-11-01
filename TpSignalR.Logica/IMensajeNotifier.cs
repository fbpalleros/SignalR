using System.Threading.Tasks;
using TpSignalR.Entidades;

namespace TpSignalR.Logica
{
    public interface IMensajeNotifier
    {
        Task NotifyMensajeAsync(Mensaje mensaje);
    }
}
