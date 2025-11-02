using System.ComponentModel.DataAnnotations;

namespace TpSignalR.Entidades
{
    public class Comercio
    {
        [Key]
        public int ComercioId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public decimal Latitud { get; set; }

        [Required]
        public decimal Longitud { get; set; }

        [MaxLength(500)]
        public string CategoriasMenu { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string PreciosMenu { get; set; } = string.Empty;

        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    
    }
}
