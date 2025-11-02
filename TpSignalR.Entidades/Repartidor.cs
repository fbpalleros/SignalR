using System.ComponentModel.DataAnnotations;
using TpSignalR.Entidades;

namespace TpSignalR.Entidades
{
    public class Repartidor
    {
        [Key]
        public int RepartidorId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;

        public decimal? LatitudActual { get; set; }
        public decimal? LongitudActual { get; set; }

        [MaxLength(50)]
        public string Estado { get; set; } = "Disponible"; // Disponible, En reparto, Descansando

        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
