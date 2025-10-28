using Microsoft.EntityFrameworkCore;
using TpSignalR.Entidades;

namespace TpSignalR.Repositorio
{
    public class InicializacionContext : DbContext
    {
        public InicializacionContext(DbContextOptions<InicializacionContext> options) : base(options)
        {
        }

        public DbSet<Comercio> Comercios { get; set; }
        public DbSet<Repartidor> Repartidores { get; set; }
        public DbSet<UsuarioFinal> UsuariosFinales { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relación Comercio-Pedido
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Comercio)
                .WithMany()
                .HasForeignKey(p => p.ComercioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Repartidor-Pedido
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Repartidor)
                .WithMany()
                .HasForeignKey(p => p.RepartidorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación UsuarioFinal-Pedido
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.UsuarioFinal)
                .WithMany()
                .HasForeignKey(p => p.UsuarioFinalId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
