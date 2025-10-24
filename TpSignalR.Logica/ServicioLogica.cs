using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TpSignalR.Entidades;
using TpSignalR.Repositorio;

namespace TpSignalR.Logica
{
    public interface IServicioLogica
    {
        List<Servicio> obtenerTodos();
        void registrarServicio(Servicio servicio);

    } 

        public class ServicioLogica : IServicioLogica
        {
            private readonly ServicioContext _context;

            private static List<Servicio> _servicio = new List<Servicio>()
            {
            };
            public ServicioLogica(ServicioContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public void registrarServicio(Servicio servicio)
            {
                if (servicio is null) throw new ArgumentNullException(nameof(servicio));
                _context.Servicios.Add(servicio);
                _context.SaveChanges();
                _servicio.Add(servicio);
            }

            public List<Servicio> obtenerTodos()
            {
                return _context.Servicios
                    .OrderBy(u => u.IdServicio)
                    .ToList();
            }

            public Servicio obtenerServicioPorID(int IdRecibido)
            {
                return _context.Servicios.Find(IdRecibido);
            }
    }
}



