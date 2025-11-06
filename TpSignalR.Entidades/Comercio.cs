using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpSignalR.Entidades
{
    public class Comercio
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        [Column("nombre")]
        public string? Nombre { get; set; }   // ← ahora acepta null

        [Column("direccion")]
        public string? Direccion { get; set; }

        [Required]
        [Column("latitud")]
        public decimal Latitud { get; set; }

        [Required]
        [Column("longitud")]
        public decimal Longitud { get; set; }

        public ICollection<Producto>? Productos { get; set; }
        public ICollection<Pedido>? Pedidos { get; set; }
    }

}
