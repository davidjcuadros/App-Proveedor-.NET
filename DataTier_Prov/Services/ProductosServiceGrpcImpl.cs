using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Productos; 
using DataTier_Prov.Models;
using DataTier_Prov.Repositories;

namespace DataTier_Prov.Services
{
    public class ProductosServiceGrpcImpl : ProductosService.ProductosServiceBase
    {
        private readonly IProductoRepository _repository;

        public ProductosServiceGrpcImpl(IProductoRepository repository)
        {
            _repository = repository;
        }

        public override async Task<CreateProductoResponse> CreateProducto(CreateProductoRequest request, ServerCallContext context)
        {
            var producto = new DataTier_Prov.Models.Producto
            {
                Id = request.Producto.Id,
                Nombre = request.Producto.Nombre,
                Descripcion = request.Producto.Descripcion,
                Cantidad = request.Producto.Cantidad
            };

            await _repository.CreateProducto(producto);

            return new CreateProductoResponse { Respuesta = "Producto creado exitosamente" };
        }

        public override async Task<GetAllProductosResponse> GetAllProductos(GetAllProductosRequest request, ServerCallContext context)
        {
            var productos = await _repository.GetAllProductos();

            var resp = new GetAllProductosResponse();
            resp.Productos.AddRange(productos.Select(p => new Productos.Producto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Cantidad = p.Cantidad
            }));

            return resp;
        }

        public override async Task<GetProductoByIdResponse> GetProductoById(GetProductoByIdRequest request, ServerCallContext context)
        {
            var p = await _repository.GetProductoById(request.Id);

            var resp = new GetProductoByIdResponse();

            if (p != null)
            {
                resp.Producto = new Productos.Producto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Cantidad = p.Cantidad
                };
            }

            return resp;
        }

        public override async Task<UpdateProductoResponse> UpdateProducto(UpdateProductoRequest request, ServerCallContext context)
        {
            var producto = new DataTier_Prov.Models.Producto
            {
                Id = request.Producto.Id,
                Nombre = request.Producto.Nombre,
                Descripcion = request.Producto.Descripcion,
                Cantidad = request.Producto.Cantidad
            };

            await _repository.UpdateProducto(producto);

            return new UpdateProductoResponse { Respuesta = "Producto actualizado" };
        }

        public override async Task<DeleteProductoResponse> DeleteProducto(DeleteProductoRequest request, ServerCallContext context)
        {
            await _repository.DeleteProducto(request.Id);

            return new DeleteProductoResponse { Respuesta = "Producto eliminado" };
        }
    }
}
