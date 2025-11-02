namespace TpSignalR.Entidades
{
    public class Partido
    {
        public int Id { get; set; }
        public string EquipoLocal { get; set; }
        public string EquipoVisitante { get; set; }
        public int AmarillasEquipoLocal { get; set; }
        public int AmarillasEquipoVisitante { get; set; }
        public int RojasEquipoLocal { get; set; }
        public int RojasEquipoVisitante { get; set; }
        public DateTime HorarioPartido { get; set; }
        public DateTime FechaCreacion { get; set; }

        // Navigation property
        public ICollection<Gol> Goles { get; set; }

        // Calculated properties (not mapped to database)
        public int GolesLocal => Goles?.Count(g => g.Equipo.ToLower() == "local") ?? 0;
        public int GolesVisitante => Goles?.Count(g => g.Equipo.ToLower() == "visitante") ?? 0;
        public string Estado
        {
            get
            {
                if (HorarioPartido > DateTime.Now)
                    return "programado";
                else if (HorarioPartido.Date == DateTime.Now.Date)
                    return "en_curso";
                else
                    return "finalizado";
            }
        }
    }
}
