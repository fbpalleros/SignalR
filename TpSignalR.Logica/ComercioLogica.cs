using System.Collections.Generic;
using System.Linq;
using TpSignalR.Entidades;
using TpSignalR.Repositorio;

namespace TpSignalR.Logica
{
    public interface IComercioLogica
    {
        List<Comercio> ObtenerTodos();
        Comercio Guardar(Comercio comercio);
    }

    public class ComercioLogica : IComercioLogica
    {
        private readonly InicializacionContext _context;

        public ComercioLogica(InicializacionContext context)
        {
            _context = context;
        }

        public List<Comercio> ObtenerTodos()
        {
            return _context.Comercios.ToList();
        }

        public Comercio Guardar(Comercio comercio)
        {
            _context.Comercios.Add(comercio);
            _context.SaveChanges();
            return comercio;
        }
    }
}
