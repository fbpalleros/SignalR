using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using TpSignalR.Entidades;

namespace TpSignalR.Repositorio
{
    public class ServicioContext : DbContext
    {
        public ServicioContext(DbContextOptions<ServicioContext> options) : base(options)
        {
        }
        public DbSet<Servicio> Servicios{ get; set; }
    }
}
