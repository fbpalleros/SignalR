namespace TpSignalR.Entidades
{
    public class Gol
    {
        public int Id { get; set; }
        public int PartidoId { get; set; }
        public int Minuto { get; set; }
        public string Jugador { get; set; }
        public string Equipo { get; set; } // 'local' o 'visitante'
        public DateTime FechaGol { get; set; }

        // Navigation property
        public Partido Partido { get; set; }
    }
}
