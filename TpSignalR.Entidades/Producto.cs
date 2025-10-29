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
        [Column("comercio_id")]
        public int ComercioId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [MaxLength(50)]
        public string Categoria { get; set; }

        [Required]
        public decimal Precio { get; set; }

        // Relación
        public Comercio Comercio { get; set; }
    }
}
