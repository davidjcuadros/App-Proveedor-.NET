using Microsoft.EntityFrameworkCore;
using ProveedorApp.Business;
using ProveedorApp.IBusiness;
using ProveedorApp.Persistance;
using Confluent.Kafka;

namespace ProveedorApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddDbContextFactory<MyAppDbContext>(
            options => options.UseNpgsql(connectionString)
        );

        builder.Services.AddScoped<IProductoBusiness, ProductoBusiness>();
        builder.Services.AddScoped<IKafkaProducer, KafkaProducer>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddHostedService<KafkaConsumer>();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // ✅ APLICAR MIGRACIONES AUTOMÁTICAMENTE
        using (var scope = app.Services.CreateScope())
        {
            var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MyAppDbContext>>();
            using var context = dbContextFactory.CreateDbContext();
            
            try
            {
                Console.WriteLine("🔄 Aplicando migraciones...");
                context.Database.Migrate();
                Console.WriteLine("✅ Migraciones aplicadas correctamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al aplicar migraciones: {ex.Message}");
                throw; // O maneja el error como prefieras
            }
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}