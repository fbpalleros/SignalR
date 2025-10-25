using System;
using System.Collections.Generic;
using System.Linq;
using TpSignalR.Entidades;
using TpSignalR.Repositorio;

namespace TpSignalR.Logica
{
    public interface IMensajeLogica
    {
        List<Mensaje> ObtenerTodos();
        Mensaje Guardar(string texto, string usuario);
    }

    public class MensajeLogica : IMensajeLogica
    {
        private readonly RegistroContext _context;

        public MensajeLogica(RegistroContext context)
        {
            _context = context;
        }

        public List<Mensaje> ObtenerTodos()
        {
            return _context.Mensajes
                .OrderBy(m => m.Fecha)
                .ToList();
        }

        public Mensaje Guardar(string texto, string usuario)
        {
            var mensaje = new Mensaje
            {
                Texto = texto,
                Usuario = usuario ?? "Anónimo",
                Fecha = DateTime.Now
            };

            _context.Mensajes.Add(mensaje);
            _context.SaveChanges();

            return mensaje;
        }
    }
}
