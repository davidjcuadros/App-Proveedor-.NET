using System;
using System.Text.Json;
using ProveedorApp.Persistance;
using ProveedorApp.IBusiness;
using ProveedorApp.Model;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ProveedorApp.Business;

public class ProductoBusiness : IProductoBusiness
{
    private readonly MyAppDbContext _context;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IConfiguration _configuration;

    //Paso principal para inyección de dependnecias
    public ProductoBusiness(MyAppDbContext context, IKafkaProducer kafkaProducer, IConfiguration configuration)
    {
        _context = context;
        _kafkaProducer = kafkaProducer;
        _configuration = configuration;
    }


    public async Task CreateProducto(Producto producto)
    {
        await _context.Productos.AddAsync(producto);
        await _context.SaveChangesAsync();

        // Enviar mensaje a Kafka
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
        return await _context.Productos.FindAsync(id);
    }

    public async Task UpdateProducto(Producto producto)
    {
        _context.Productos.Update(producto);
        await _context.SaveChangesAsync();

        // Enviar mensaje a Kafka
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
        var producto = await _context.Productos.FindAsync(id);
        if (producto != null)
        {
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            // Enviar mensaje a Kafka
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
