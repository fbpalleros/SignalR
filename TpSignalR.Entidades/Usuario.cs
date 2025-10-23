using System.ComponentModel.DataAnnotations;

namespace TpSignalR.Entidades
{
    public class Usuario
    {

        [Key]
        public int id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; }

       
        public int IdUsuario { get; set; }

    }
}
