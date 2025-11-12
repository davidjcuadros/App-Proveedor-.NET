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

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Registrar EF Core
            builder.Services.AddDbContextFactory<MyAppDbContext>(
                options => options.UseNpgsql(connectionString)
            );

            // Registrar Repos y Kafka
            builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
            builder.Services.AddScoped<IKafkaProducer, KafkaProducer>();
            builder.Services.AddHostedService<KafkaConsumer>();
            builder.Services.AddSingleton<EmailService>();

            // Registrar gRPC Server
            builder.Services.AddGrpc();

            var app = builder.Build();

            // Migraciones automáticas
            using (var scope = app.Services.CreateScope())
            {
                var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MyAppDbContext>>();
                using var db = factory.CreateDbContext();
                db.Database.Migrate();
            }

            // Exponer gRPC
            app.MapGrpcService<ProductosServiceGrpcImpl>();

            app.MapGet("/", () => "DataTier_Prov gRPC server running.");

            app.Run();
        }
    }
}
