using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using DataTier_Prov.Models;
using DataTier_Prov.Persistance;

namespace DataTier_Prov.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly MyAppDbContext _context;
        private readonly IKafkaProducer _kafkaProducer;
        private readonly IConfiguration _configuration;

        public ProductoRepository(MyAppDbContext context, IKafkaProducer kafkaProducer, IConfiguration configuration)
        {
            _context = context;
            _kafkaProducer = kafkaProducer;
            _configuration = configuration;
        }

        private string GetKafkaTopic()
        {
            return _configuration["Kafka:Topic"] ?? "productos-default";
        }

        public async Task CreateProducto(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            // Kafka: enviar mensaje con formato de correo
            var message = JsonSerializer.Serialize(new
            {
                To = producto.Correo ?? _configuration["Email:NotificationTo"],
                Subject = $"🆕 Nuevo producto registrado: {producto.Nombre}",
                Body = $@"
                    <h3>Se ha registrado un nuevo producto</h3>
                    <p><strong>Nombre:</strong> {producto.Nombre}</p>
                    <p><strong>Descripción:</strong> {producto.Descripcion}</p>
                    <p><strong>Cantidad:</strong> {producto.Cantidad}</p>
                    <p>Fecha de registro: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}</p>",
                Timestamp = DateTime.UtcNow
            });

            await _kafkaProducer.ProduceAsync(GetKafkaTopic(), message);
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

            var message = JsonSerializer.Serialize(new
            {
                To = producto.Correo ?? _configuration["Email:NotificationTo"],
                Subject = $"✏️ Producto actualizado: {producto.Nombre}",
                Body = $@"
                    <h3>Se ha actualizado un producto existente</h3>
                    <p><strong>Nombre:</strong> {producto.Nombre}</p>
                    <p><strong>Descripción:</strong> {producto.Descripcion}</p>
                    <p><strong>Cantidad actualizada:</strong> {producto.Cantidad}</p>
                    <p>Fecha de actualización: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}</p>",
                Timestamp = DateTime.UtcNow
            });

            await _kafkaProducer.ProduceAsync(GetKafkaTopic(), message);
        }

        public async Task DeleteProducto(int id)
        {
            var producto = await GetProductoById(id);

            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();

                var message = JsonSerializer.Serialize(new
                {
                    To = producto.Correo ?? _configuration["Email:NotificationTo"],
                    Subject = $"🗑️ Producto eliminado: {producto.Nombre}",
                    Body = $@"
                        <h3>Un producto ha sido eliminado del inventario</h3>
                        <p><strong>Nombre:</strong> {producto.Nombre}</p>
                        <p><strong>Descripción:</strong> {producto.Descripcion}</p>
                        <p><strong>Cantidad eliminada:</strong> {producto.Cantidad}</p>
                        <p>Fecha de eliminación: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}</p>",
                    Timestamp = DateTime.UtcNow
                });

                await _kafkaProducer.ProduceAsync(GetKafkaTopic(), message);
            }
        }
    }
}
