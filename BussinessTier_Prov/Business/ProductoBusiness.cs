using System.Linq;
using BussinessTier_Prov.IBusiness;
using BussinessTier_Prov.Models;
using Productos;

namespace BussinessTier_Prov.Business
{
    public class ProductoBusiness : IProductoBusiness
    {
        private readonly ProductosService.ProductosServiceClient _client;

        public ProductoBusiness(ProductosService.ProductosServiceClient client)
        {
            _client = client;
        }

        public async Task CreateProducto(Models.Producto producto)
        {
            var request = new CreateProductoRequest
            {
                Producto = new Productos.Producto
                {
                    Id = producto.Id,
                    Nombre = producto.Nombre,
                    Descripcion = producto.Descripcion,
                    Cantidad = producto.Cantidad,
                    Correo = producto.Correo
                }
            };

            await _client.CreateProductoAsync(request);
        }

        public async Task<List<Models.Producto>> GetAllProductos()
        {
            var response = await _client.GetAllProductosAsync(new GetAllProductosRequest());

            return response.Productos.Select(p => new Models.Producto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Cantidad = p.Cantidad,
                Correo = p.Correo
            }).ToList();
        }

        public async Task<Models.Producto?> GetProductoById(int id)
        {
            var response = await _client.GetProductoByIdAsync(new GetProductoByIdRequest { Id = id });

            if (response.Producto == null)
                return null;

            return new Models.Producto
            {
                Id = response.Producto.Id,
                Nombre = response.Producto.Nombre,
                Descripcion = response.Producto.Descripcion,
                Cantidad = response.Producto.Cantidad,
                Correo = response.Producto.Correo
            };
        }

        public async Task UpdateProducto(Models.Producto producto)
        {
            var request = new UpdateProductoRequest
            {
                Producto = new Productos.Producto
                {
                    Id = producto.Id,
                    Nombre = producto.Nombre,
                    Descripcion = producto.Descripcion,
                    Cantidad = producto.Cantidad,
                    Correo = producto.Correo
                }
            };

            await _client.UpdateProductoAsync(request);
        }

        public async Task DeleteProducto(int id)
        {
            await _client.DeleteProductoAsync(new DeleteProductoRequest { Id = id });
        }
    }
}
