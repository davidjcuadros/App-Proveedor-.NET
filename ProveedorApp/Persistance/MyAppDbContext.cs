using System;

namespace ProveedorApp.Persistance;

using Microsoft.EntityFrameworkCore;
using ProveedorApp.Model;

public class MyAppDbContext : DbContext
{
    public MyAppDbContext(DbContextOptions options) : base(options)
    {
        //necesario para crear mi bd
    }
    public DbSet<Producto> Productos { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>().HasKey(c => c.Id);
        modelBuilder.Entity<Producto>().Property(c => c.Descripcion).HasMaxLength(256);
        modelBuilder.Entity<Producto>().Property(c => c.Nombre).IsRequired();
    }

}
