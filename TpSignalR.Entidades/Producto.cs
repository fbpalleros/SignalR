using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpSignalR.Entidades
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Comercio")]
        public int ComercioId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Categoria { get; set; } = string.Empty;

        [Required]
        public decimal Precio { get; set; }

        // Relación
        public Comercio Comercio { get; set; } = null!;
    }
}
