using System.ComponentModel.DataAnnotations;

namespace TpSignalR.Entidades
{
    public class Usuario
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; }

        public string Rol { get; set; }

        public double Latitud { get; set; }
        public double Longitud { get; set; }
    }
}
