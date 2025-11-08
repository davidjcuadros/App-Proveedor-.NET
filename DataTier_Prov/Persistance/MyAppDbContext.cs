using System;

namespace ProveedorApp.Persistance;

using Microsoft.EntityFrameworkCore;
using DataTier_Prov.Models;

public class MyAppDbContext : DbContext
{
    public MyAppDbContext(DbContextOptions options) : base(options)
    {
        //necesario para crear mi bd
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("producto");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre).HasColumnName("nombre");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
        });

    }

    public DbSet<Producto> Productos { get; set; }


}
