using System.ComponentModel.DataAnnotations;

namespace TpSignalR.Entidades
{
    public class Usuario
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; } = string.Empty;

        public float Latitud { get; set; }
        public float Longitud { get; set; }
    }
}
