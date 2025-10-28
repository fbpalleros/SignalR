using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpSignalR.Entidades
{
    public class Pedido
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("UsuarioFinal")]
        public int UsuarioId { get; set; }

        [Required]
        [ForeignKey("Comercio")]
        public int ComercioId { get; set; }

        [ForeignKey("Repartidor")]
        public int? RepartidorId { get; set; }

        public DateTime FechaPedido { get; set; } = DateTime.Now;

        [MaxLength(20)]
        public string Estado { get; set; } = "pendiente";

        public decimal Total { get; set; }

        // Relaciones
        public UsuarioFinal UsuarioFinal { get; set; }
        public Comercio Comercio { get; set; }
        public Repartidor Repartidor { get; set; }

        public ICollection<PedidoDetalle> Detalles { get; set; }
    }
}
