using Microsoft.EntityFrameworkCore;
using DataTier_Prov.Models;

namespace DataTier_Prov.Persistance
{
    public class MyAppDbContext : DbContext
    {
        public MyAppDbContext(DbContextOptions<MyAppDbContext> options) : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("producto");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Descripcion).HasColumnName("descripcion");
                entity.Property(e => e.Cantidad).HasColumnName("cantidad");
                entity.Property(e => e.Correo).HasColumnName("correo");

            });
        }
    }
}
