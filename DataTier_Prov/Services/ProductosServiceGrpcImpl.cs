using System;
using Grpc.Core;
using Productos; // namespace generado por el proto
using DataTier_Prov.Models; // entidades EF
using ProveedorApp.Persistance; // tu namespace
using DataTier_Prov.Repositories; // donde esté tu IProductoRepository
namespace DataTier_Prov.Services;

public class ProductosServiceGrpcImpl: ProductosService.ProductosServiceBase
{
    private readonly IProductoRepository _repository;

    public ProductosServiceGrpcImpl(IProductoRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CreateProductoResponse> CreateProducto(CreateProductoRequest request, ServerCallContext context)
    {
        var producto = new Models.Producto
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

        var response = new GetAllProductosResponse();
        response.Productos.AddRange(productos.Select(p => new Productos.Producto
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            Cantidad = p.Cantidad
        }));

        return response;
    }

    public override async Task<GetProductoByIdResponse> GetProductoById(GetProductoByIdRequest request, ServerCallContext context)
    {
        var producto = await _repository.GetProductoById(request.Id);

        return new GetProductoByIdResponse
        {
            Producto = producto == null ? null : new Productos.Producto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Cantidad = producto.Cantidad
            }
        };
    }

    public override async Task<UpdateProductoResponse> UpdateProducto(UpdateProductoRequest request, ServerCallContext context)
    {
        var producto = new Models.Producto
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