using System.ComponentModel.DataAnnotations;

namespace TpSignalR.Entidades
{
    public class UsuarioFinal
    {
        [Key]
        public int UsuarioFinalId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(200)]
        public string Direccion { get; set; }

        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }

        [MaxLength(500)]
        public string Pedido { get; set; } // Descripción breve del pedido

        public ICollection<Pedido> Pedidos { get; set; }
    }
}
