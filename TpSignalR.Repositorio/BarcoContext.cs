using Microsoft.EntityFrameworkCore;
using TpSignalR.Entidades;

namespace TpSignalR.Repositorio
{
    public class BarcoContext : DbContext
    {
        public BarcoContext(DbContextOptions<BarcoContext> options) : base(options)
        {
        }
        public DbSet<Barco> Barcos { get; set; }
    }
}
