using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using DataTier_Prov.Models;
using ProveedorApp.Persistance;
using ProveedorApp.IBusiness;

namespace DataTier_Prov.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly MyAppDbContext _context;
        private readonly IKafkaProducer _kafkaProducer;
        private readonly IConfiguration _configuration;

        // Constructor con inyección de dependencias
        public ProductoRepository(MyAppDbContext context, IKafkaProducer kafkaProducer, IConfiguration configuration)
        {
            _context = context;
            _kafkaProducer = kafkaProducer;
            _configuration = configuration;
        }

        public async Task CreateProducto(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            // Evento Kafka
            var message = JsonSerializer.Serialize(new
            {
                Action = "Created",
                Producto = producto,
                Timestamp = DateTime.UtcNow
            });

            var topic = _configuration["Kafka:Topic"];
            await _kafkaProducer.ProduceAsync(topic, message);
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

            // Evento Kafka
            var message = JsonSerializer.Serialize(new
            {
                Action = "Updated",
                Producto = producto,
                Timestamp = DateTime.UtcNow
            });

            var topic = _configuration["Kafka:Topic"];
            await _kafkaProducer.ProduceAsync(topic, message);
        }

        public async Task DeleteProducto(int id)
        {
            var producto = await GetProductoById(id);

            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();

                // Evento Kafka
                var message = JsonSerializer.Serialize(new
                {
                    Action = "Deleted",
                    ProductoId = id,
                    Timestamp = DateTime.UtcNow
                });

                var topic = _configuration["Kafka:Topic"];
                await _kafkaProducer.ProduceAsync(topic, message);
            }
        }
    }
}
