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
        private readonly ServicioRepartoContext _context;
        private readonly IMensajeNotifier _notifier;

        public MensajeLogica(ServicioRepartoContext context, IMensajeNotifier notifier = null)
        {
            _context = context;
            _notifier = notifier;
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

            // Notify asynchronously if notifier provided (fire-and-forget safe)
            if (_notifier != null)
            {
                _ = _notifier.NotifyMensajeAsync(mensaje);
            }

            return mensaje;
        }
    }
}
