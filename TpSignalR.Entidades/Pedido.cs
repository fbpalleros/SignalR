using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TpSignalR.Entidades
{
    public class Pedido
    {
        [Key]
        [Column("id")]
        public int PedidoId { get; set; }

        [ForeignKey("Comercio")]
        [Column("comercio_id")]
        public int ComercioId { get; set; }

        [ForeignKey("Repartidor")]
        [Column("repartidor_id")]
        public int? RepartidorId { get; set; }

        [ForeignKey("UsuarioFinal")]
        [Column("usuario_id")]
        public int UsuarioFinalId { get; set; }

        [Column("producto_id")]
        public int ProductoId { get; set; }


        public decimal Total { get; set; }

        [MaxLength(50)]
        public string Estado { get; set; } // Pendiente, En camino, Entregado

        // Relaciones
        public Comercio Comercio { get; set; }
        public Repartidor Repartidor { get; set; }
        public UsuarioFinal UsuarioFinal { get; set; }
        public ICollection<PedidoDetalle> Detalles { get; set; }
    }
}
