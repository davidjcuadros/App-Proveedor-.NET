using System;
using ProveedorApp.Persistance;
using ProveedorApp.IBusiness;
using ProveedorApp.Model;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;

namespace ProveedorApp.Business;

public class ProductoBusiness : IProductoBusiness
{
    private readonly MyAppDbContext _context;

    //Paso principal para inyección de dependnecias
    public ProductoBusiness(MyAppDbContext context)
    {
        _context = context;
    }


    public async Task CreateProducto(Producto producto)
    {
        await _context.Productos.AddAsync(producto);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Producto>> GetAllProductos()
    {
        return await _context.Productos.ToListAsync();
    }

    public async Task<Producto?> GetProductoById(int id)
    {
        return await _context.Productos.FindAsync(id);
    }

    public async Task UpdateProducto(Producto producto)
    {
        _context.Productos.Update(producto);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProducto(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto != null)
        {
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
        }
    }
}
