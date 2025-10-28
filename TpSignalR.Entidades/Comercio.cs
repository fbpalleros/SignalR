using System.ComponentModel.DataAnnotations;

namespace TpSignalR.Entidades
{
    public class Comercio
    {
        [Key]
        public int ComercioId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        public decimal Latitud { get; set; }

        [Required]
        public decimal Longitud { get; set; }

        [MaxLength(500)]
        public string CategoriasMenu { get; set; }

        [MaxLength(1000)]
        public string PreciosMenu { get; set; }

        public ICollection<Producto> Productos { get; set; }
        public ICollection<Pedido> Pedidos { get; set; }
    
    }
}
