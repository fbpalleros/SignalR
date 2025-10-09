using Microsoft.EntityFrameworkCore;
using RegistroAduanero.Entidades;

namespace RegistroAduanero.Repositorio
{
    public class BarcoContext : DbContext
    {
        public BarcoContext(DbContextOptions<BarcoContext> options) : base(options)
        {
        }
        public DbSet<Barco> Barcos { get; set; }
    }
}
