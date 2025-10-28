using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TpSignalR.Entidades
{
    public class UsuarioFinal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(150)]
        public string Direccion { get; set; }

        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }

        // Relaciones
        public ICollection<Pedido> Pedidos { get; set; }
    }
}
