using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TpSignalR.Entidades
{
    public class Comercio
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [MaxLength(150)]
        public string Direccion { get; set; }

        [Required]
        public decimal Latitud { get; set; }

        [Required]
        public decimal Longitud { get; set; }

        // Relaciones
        public ICollection<Producto> Productos { get; set; }
        public ICollection<Pedido> Pedidos { get; set; }
    }
}
