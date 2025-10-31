using System;
using ProveedorApp.Model; 

namespace ProveedorApp.IBusiness;

public interface IProductoBusiness
{
    Task CreateProducto(Producto producto);
    Task<List<Producto>> GetAllProductos();
    Task<Producto?> GetProductoById(int id);
    Task UpdateProducto(Producto producto);
    Task DeleteProducto(int id);
}
