using System;

namespace BussinessTier_Prov.IBusiness;

using BussinessTier_Prov.Models;

public interface IProductoBusiness
{
    Task CreateProducto(Producto producto);
    Task<List<Producto>> GetAllProductos();
    Task<Producto?> GetProductoById(int id);
    Task UpdateProducto(Producto producto);
    Task DeleteProducto(int id);
}
