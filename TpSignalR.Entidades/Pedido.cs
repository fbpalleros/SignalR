using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpSignalR.Entidades
{
    public class Pedido
    {
        [Key]
        public int PedidoId { get; set; }

        [ForeignKey("Comercio")]
        public int ComercioId { get; set; }

        [ForeignKey("Repartidor")]
        public int RepartidorId { get; set; }

        [ForeignKey("UsuarioFinal")]
        public int UsuarioFinalId { get; set; }

        [MaxLength(500)]
        public string Descripcion { get; set; }

        public decimal Total { get; set; }

        [MaxLength(50)]
        public string Estado { get; set; } // Pendiente, En camino, Entregado

        public DateTime HoraInicio { get; set; }
        public DateTime? HoraEntrega { get; set; }

        public Comercio Comercio { get; set; }
        public Repartidor Repartidor { get; set; }
        public UsuarioFinal UsuarioFinal { get; set; }
        public ICollection<PedidoDetalle> Detalles { get; set; }
    }
}
