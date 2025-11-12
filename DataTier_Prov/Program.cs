using Microsoft.EntityFrameworkCore;
using DataTier_Prov.Persistance;
using DataTier_Prov.Repositories;
using DataTier_Prov.Services;
using DataTier_Prov.Mail;

namespace DataTier_Prov
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddEnvironmentVariables();

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContextFactory<MyAppDbContext>(
                options => options.UseNpgsql(connectionString)
            );

            // Registrar repositorios, servicios y Kafka producer/consumer
            builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
            builder.Services.AddScoped<IKafkaProducer, KafkaProducer>();

            // Email y Kafka consumer 
            builder.Services.AddSingleton<EmailService>();
            builder.Services.AddHostedService<KafkaConsumer>();

            builder.Services.AddGrpc();

            builder.WebHost.UseUrls("http://0.0.0.0:8080");

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MyAppDbContext>>();
                using var db = factory.CreateDbContext();
                db.Database.Migrate();
            }

            app.MapGrpcService<ProductosServiceGrpcImpl>();

            app.MapGet("/", () => "✅ DataTier_Prov gRPC server running with Kafka & Email integration.");

            app.Run();
        }
    }
}
