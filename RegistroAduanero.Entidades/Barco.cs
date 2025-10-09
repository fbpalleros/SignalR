using System.ComponentModel.DataAnnotations;

namespace RegistroAduanero.Entidades
{
    public class Barco
    {

        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; }

        [Range(0,500)]
        public int Antiguedad { get; set; }

        [Range(1,20000)]
        public int TripulacionMaxima { get; set; }
        public double Impuesto { get; set; }


    }
}
