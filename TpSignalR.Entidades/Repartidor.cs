using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpSignalR.Entidades
{
    public class Repartidor
    {
        [Key]
        [Column("id")]
        public int RepartidorId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("latitud_actual")]
        public decimal? LatitudActual { get; set; }

        [Column("longitud_actual")]
        public decimal? LongitudActual { get; set; }

        [MaxLength(50)]
        [Column("estado")]
        public string Estado { get; set; } // Disponible, En reparto, Descansando

        public ICollection<Pedido> Pedidos { get; set; }
    }
}
