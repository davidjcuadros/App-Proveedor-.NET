using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Grpc.Core;
using Productos;
using DataTier_Prov.Models;
using DataTier_Prov.Repositories;
using Microsoft.Extensions.Configuration;

namespace DataTier_Prov.Services
{
    public class ProductosServiceGrpcImpl : ProductosService.ProductosServiceBase
    {
        private readonly IProductoRepository _repository;
        private readonly IKafkaProducer _kafkaProducer;
        private readonly IConfiguration _configuration;

        public ProductosServiceGrpcImpl(IProductoRepository repository, IKafkaProducer kafkaProducer, IConfiguration configuration)
        {
            _repository = repository;
            _kafkaProducer = kafkaProducer;
            _configuration = configuration;
        }

        // --- CREATE ---
        public override async Task<CreateProductoResponse> CreateProducto(CreateProductoRequest request, ServerCallContext context)
        {
            var producto = new Models.Producto
            {
                Id = request.Producto.Id,
                Nombre = request.Producto.Nombre,
                Descripcion = request.Producto.Descripcion,
                Cantidad = request.Producto.Cantidad,
                Correo = request.Producto.Correo
            };

            await _repository.CreateProducto(producto);

            // Mensaje de correo
            var emailData = new
            {
                To = producto.Correo,
                Subject = "✅ Producto creado exitosamente",
                Body = $"<h2>Producto creado</h2><p><strong>Nombre:</strong> {producto.Nombre}</p><p><strong>Descripción:</strong> {producto.Descripcion}</p><p><strong>Cantidad:</strong> {producto.Cantidad}</p>"
            };

            await EnviarMensajeKafka(emailData);

            return new CreateProductoResponse { Respuesta = "Producto creado exitosamente" };
        }

        // --- GET ALL ---
        public override async Task<GetAllProductosResponse> GetAllProductos(GetAllProductosRequest request, ServerCallContext context)
        {
            var productos = await _repository.GetAllProductos();

            var resp = new GetAllProductosResponse();
            resp.Productos.AddRange(productos.Select(p => new Productos.Producto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Cantidad = p.Cantidad,
                Correo = p.Correo
            }));

            return resp;
        }

        // --- GET BY ID ---
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
                    Cantidad = p.Cantidad,
                    Correo = p.Correo
                };
            }

            return resp;
        }

        // --- UPDATE ---
        public override async Task<UpdateProductoResponse> UpdateProducto(UpdateProductoRequest request, ServerCallContext context)
        {
            var producto = new Models.Producto
            {
                Id = request.Producto.Id,
                Nombre = request.Producto.Nombre,
                Descripcion = request.Producto.Descripcion,
                Cantidad = request.Producto.Cantidad,
                Correo = request.Producto.Correo
            };

            await _repository.UpdateProducto(producto);

            var emailData = new
            {
                To = producto.Correo,
                Subject = "✏️ Producto actualizado",
                Body = $"<h2>Actualización de producto</h2><p><strong>Nombre:</strong> {producto.Nombre}</p><p><strong>Descripción:</strong> {producto.Descripcion}</p><p><strong>Cantidad:</strong> {producto.Cantidad}</p>"
            };

            await EnviarMensajeKafka(emailData);

            return new UpdateProductoResponse { Respuesta = "Producto actualizado exitosamente" };
        }

        // --- DELETE ---
        public override async Task<DeleteProductoResponse> DeleteProducto(DeleteProductoRequest request, ServerCallContext context)
        {
            var producto = await _repository.GetProductoById(request.Id);

            if (producto != null)
            {
                await _repository.DeleteProducto(request.Id);

                var emailData = new
                {
                    To = producto.Correo,
                    Subject = "🗑️ Producto eliminado",
                    Body = $"<h2>Producto eliminado</h2><p><strong>Nombre:</strong> {producto.Nombre}</p><p><strong>Descripción:</strong> {producto.Descripcion}</p>"
                };

                await EnviarMensajeKafka(emailData);
            }

            return new DeleteProductoResponse { Respuesta = "Producto eliminado" };
        }

        // --- MÉTODO AUXILIAR ---
        private async Task EnviarMensajeKafka(object emailData)
        {
            var topic = _configuration["Kafka:Topic"];
            var message = JsonSerializer.Serialize(emailData);
            await _kafkaProducer.ProduceAsync(topic, message);
        }
    }
}
