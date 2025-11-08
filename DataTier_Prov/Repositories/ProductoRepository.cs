using System;
using DataTier_Prov.Models;
using Microsoft.EntityFrameworkCore;
using ProveedorApp.Persistance;

namespace DataTier_Prov.Repositories;

public class ProductoRepository : IProductoRepository
{

    private readonly MyAppDbContext _context;

    public ProductoRepository(MyAppDbContext context)
    {
        _context = context;
    }

    public async Task CreateProducto(Producto producto)
    {
        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Producto>> GetAllProductos()
    {
        return await _context.Productos.ToListAsync();
    }

    public async Task<Producto?> GetProductoById(int id)
    {
        return await _context.Productos.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task UpdateProducto(Producto producto)
    {
        _context.Productos.Update(producto);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProducto(int id)
    {
        var p = await GetProductoById(id);
        if (p != null)
        {
            _context.Productos.Remove(p);
            await _context.SaveChangesAsync();
        }
    }
}
