using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class ComercioHub : Hub
{
    public async Task CambiarEstadoPedido(int id, string nuevoEstado)
    {
        // 🔹 Podés loguear o actualizar la BDD más adelante.
        Console.WriteLine($"🟢 Pedido {id} cambiado a estado: {nuevoEstado}");

        // 🔹 Notificá a todos los clientes conectados (por ejemplo, al panel del repartidor)
        await Clients.All.SendAsync("PedidoActualizado", id, nuevoEstado);
    }
}
