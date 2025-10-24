using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace TpSignalR.Entidades
{
        public class Servicio
        {

            [Key]
            public int IdServicio { get; set; }

            [Required]
            [MaxLength(50)]
            public string Nombre { get; set; }

        }
    

}
