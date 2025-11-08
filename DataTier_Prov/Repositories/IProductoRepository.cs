using System;
using DataTier_Prov.Models;

namespace DataTier_Prov.Repositories;

public interface IProductoRepository
{
    Task CreateProducto(Producto producto);
    Task<List<Producto>> GetAllProductos();
    Task<Producto?> GetProductoById(int id);
    Task UpdateProducto(Producto producto);
    Task DeleteProducto(int id);
}

