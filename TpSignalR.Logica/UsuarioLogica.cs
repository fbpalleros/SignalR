using System;
using System.Collections.Generic;
using System.Linq;
using TpSignalR.Entidades;
using TpSignalR.Repositorio;


namespace TpSignalR.Logica
{
    public interface IUsuarioLogica
    {
        List<Usuario> obtenerTodos();
        void registrarUsuario(Usuario usuario);
    }

    public class UsuarioLogica : IUsuarioLogica
    {
        private readonly RegistroContext _context;

        private static List<Usuario> _usuario = new List<Usuario>()
        {
        };
        public UsuarioLogica(RegistroContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void registrarUsuario(Usuario usuario)
        {
            if (usuario is null) throw new ArgumentNullException(nameof(usuario));
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
            _usuario.Add(usuario);
        }

        public List<Usuario> obtenerTodos()
        {
            return _context.Usuarios
                .OrderBy(u => u.Id)
                .ToList();
        }
    }
}

