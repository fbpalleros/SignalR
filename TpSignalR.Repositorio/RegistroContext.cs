using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using TpSignalR.Entidades;

namespace TpSignalR.Repositorio
{
    public class RegistroContext : DbContext
    {
        public RegistroContext(DbContextOptions<RegistroContext> options) : base(options)
        {
        }
        public DbSet<Usuario> Usuarios{ get; set; }
    }
}
