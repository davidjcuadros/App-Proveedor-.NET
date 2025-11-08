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
    public DbSet<Producto> Productos { get; set; }


}
