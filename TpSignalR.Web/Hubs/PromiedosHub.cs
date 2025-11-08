using Microsoft.AspNetCore.SignalR;

namespace TpSignalR.Web.Hubs
{
    public class PromiedosHub : Hub
    {
        public async Task ActualizarTarjetas(int partidoId, string equipo, int amarillas, int rojas)
        {
            await Clients.All.SendAsync("TarjetasActualizadas", partidoId, equipo, amarillas, rojas);
        }

        public async Task ActualizarGoles(int partidoId, int golesLocal, int golesVisitante)
        {
            await Clients.All.SendAsync("GolesActualizados", partidoId, golesLocal, golesVisitante);
        }

        public async Task NuevoGol(int partidoId, string equipo, string jugador, int minuto, int golesLocal, int golesVisitante)
        {
            await Clients.All.SendAsync("GolRegistrado", partidoId, equipo, jugador, minuto, golesLocal, golesVisitante);
        }
    }
}
