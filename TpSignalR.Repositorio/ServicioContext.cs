using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using TpSignalR.Entidades;

namespace TpSignalR.Repositorio
{
    public class ServicioContext : DbContext
    {
        public ServicioContext(DbContextOptions<RegistroContext> options) : base(options)
        {
        }
        public DbSet<Servicio> Servicios{ get; set; }
    }
}
