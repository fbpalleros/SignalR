using System;
using System.Collections.Generic;
using System.Linq;
using TpSignalR.Entidades;
using TpSignalR.Repositorio;


namespace TpSignalR.Logica
{
    public interface IRegistroLogica
    {
        List<Usuario> obtenerTodos();
        void registrarUsuario(Usuario usuario);
    }

    public class RegistroLogica : IRegistroLogica
    {
        private readonly RegistroContext _context;

        public RegistroLogica(RegistroContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void registrarUsuario(Usuario usuario)
        {
            if (usuario is null) throw new ArgumentNullException(nameof(usuario));
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
        }

        public List<Usuario> obtenerTodos()
        {
            return _context.Usuarios
                .OrderBy(u => u.IdUsuario)
                .ToList();
        }
    }
}

