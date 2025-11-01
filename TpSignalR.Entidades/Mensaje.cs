using System;
using System.ComponentModel.DataAnnotations;

namespace TpSignalR.Entidades
{
    public class Mensaje
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Texto { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [MaxLength(50)]
        public string Usuario { get; set; }
    }
}
