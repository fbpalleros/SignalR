using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpSignalR.Entidades
{
    public class PedidoDetalle
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Pedido")]
        public int PedidoId { get; set; }

        [Required]
        [ForeignKey("Producto")]
        public int ProductoId { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public decimal PrecioUnitario { get; set; }

        // Relaciones
        public Pedido Pedido { get; set; }
        public Producto Producto { get; set; }
    }
}
