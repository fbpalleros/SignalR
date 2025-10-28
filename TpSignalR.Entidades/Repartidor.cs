using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TpSignalR.Entidades
{
    public class Repartidor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        public decimal? LatitudActual { get; set; }
        public decimal? LongitudActual { get; set; }

        [MaxLength(20)]
        public string Estado { get; set; } = "disponible";

        // Relaciones
        public ICollection<Pedido> Pedidos { get; set; }
    }
}
