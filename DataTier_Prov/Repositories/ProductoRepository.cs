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
            // Evita warning CS8604
            return _configuration["Kafka:Topic"] ?? "productos-default";
        }

        public async Task CreateProducto(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            var message = JsonSerializer.Serialize(new
            {
                Action = "Created",
                Producto = producto,
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
                Action = "Updated",
                Producto = producto,
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
                    Action = "Deleted",
                    ProductoId = id,
                    Timestamp = DateTime.UtcNow
                });

                await _kafkaProducer.ProduceAsync(GetKafkaTopic(), message);
            }
        }
    }
}
