using Microsoft.EntityFrameworkCore;
using TpSignalR.Entidades;

namespace TpSignalR.Repositorio
{
    public class ServicioRepartoContext : DbContext
    {
        public ServicioRepartoContext(DbContextOptions<ServicioRepartoContext> options)
            : base(options)
        {
        }

        // ==========================
        // TABLAS (DbSet)
        // ==========================
        public DbSet<Comercio> Comercio { get; set; }
        public DbSet<Producto> Producto { get; set; }
        public DbSet<Repartidor> Repartidor { get; set; }
        public DbSet<UsuarioFinal> UsuarioFinal { get; set; }
        public DbSet<Pedido> Pedido { get; set; }
        public DbSet<PedidoDetalle> PedidoDetalle { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Mensaje> Mensajes { get; set; }
        public DbSet<Partido> Partidos { get; set; }
        public DbSet<Gol> Goles { get; set; }
        // ==========================
        // CONFIGURACIÓN DEL MODELO
        // ==========================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---- COMERCIO ----
            modelBuilder.Entity<Comercio>()
                .HasMany(c => c.Productos)
                .WithOne(p => p.Comercio)
                .HasForeignKey(p => p.ComercioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comercio>()
                .HasMany(c => c.Pedidos)
                .WithOne(p => p.Comercio)
                .HasForeignKey(p => p.ComercioId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---- PRODUCTO ----
            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasColumnType("decimal(10,2)");

            // ---- REPARTIDOR ----
            modelBuilder.Entity<Repartidor>()
                .HasMany(r => r.Pedidos)
                .WithOne(p => p.Repartidor)
                .HasForeignKey(p => p.RepartidorId)
                .OnDelete(DeleteBehavior.SetNull);

            // ---- USUARIO FINAL ----
            modelBuilder.Entity<UsuarioFinal>()
                .HasMany(u => u.Pedidos)
                .WithOne(p => p.UsuarioFinal)
                .HasForeignKey(p => p.UsuarioFinalId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---- PEDIDO ----
            modelBuilder.Entity<Pedido>()
                .Property(p => p.Total)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Pedido>()
                .HasMany(p => p.Detalles)
                .WithOne(d => d.Pedido)
                .HasForeignKey(d => d.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---- PEDIDO DETALLE ----
            modelBuilder.Entity<PedidoDetalle>()
                .Property(d => d.PrecioUnitario)
                .HasColumnType("decimal(10,2)");

            // ---- MENSAJE ----
            modelBuilder.Entity<Mensaje>()
                .Property(m => m.Texto)
                .HasMaxLength(500);

            // ---- PARTIDO ----
            modelBuilder.Entity<Partido>()
                .HasMany(p => p.Goles)
                .WithOne(g => g.Partido)
                .HasForeignKey(g => g.PartidoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ignore calculated properties
            modelBuilder.Entity<Partido>()
                .Ignore(p => p.GolesLocal)
                .Ignore(p => p.GolesVisitante)
                .Ignore(p => p.Estado);

            // ---- GOL ----
            modelBuilder.Entity<Gol>()
                .Property(g => g.Equipo)
                .HasMaxLength(20);
        }
    }
}
