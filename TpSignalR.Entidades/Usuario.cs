using System.ComponentModel.DataAnnotations;

namespace TpSignalR.Entidades
{
    public class Usuario
    {

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; }

        [Key]
        public int IdUsuario { get; set; }

    }
}
