using System.ComponentModel.DataAnnotations;

namespace TpSignalR.Entidades
{
    public class Repartidor
    {
        [Key]
        public int RepartidorId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [MaxLength(20)]
        public string Telefono { get; set; }

        public decimal? LatitudActual { get; set; }
        public decimal? LongitudActual { get; set; }

        [MaxLength(50)]
        public string Estado { get; set; } // Disponible, En reparto, Descansando

        public ICollection<Pedido> Pedidos { get; set; }
    }
}
