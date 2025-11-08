using Microsoft.EntityFrameworkCore;
using TpSignalR.Entidades;
using TpSignalR.Repositorio;

namespace TpSignalR.Logica
{
    public interface IPartidoLogica
    {
        List<Partido> ObtenerTodosLosPartidos();
        Partido ObtenerPartidoPorId(int id);
        List<Gol> ObtenerGolesPorPartido(int partidoId);
        void ActualizarPartido(Partido partido);
        void RegistrarGol(Gol gol);
    }

    public class PartidoLogica : IPartidoLogica
    {
        private readonly ServicioRepartoContext _context;

        public PartidoLogica(ServicioRepartoContext context)
        {
            _context = context;
        }

        public List<Partido> ObtenerTodosLosPartidos()
        {
            return _context.Partidos
                .Include(p => p.Goles) // Include goals to calculate scores
                .OrderBy(p => p.HorarioPartido)
                .ToList();
        }

        public Partido ObtenerPartidoPorId(int id)
        {
            return _context.Partidos
                .Include(p => p.Goles) // Include goals to calculate scores
                .FirstOrDefault(p => p.Id == id);
        }

        public List<Gol> ObtenerGolesPorPartido(int partidoId)
        {
            return _context.Goles
                .Where(g => g.PartidoId == partidoId)
                .OrderBy(g => g.Minuto)
                .ToList();
        }

        public void ActualizarPartido(Partido partido)
        {
            _context.Partidos.Update(partido);
            _context.SaveChanges();
        }

        public void RegistrarGol(Gol gol)
        {
            _context.Goles.Add(gol);
            _context.SaveChanges();
        }
    }
}
