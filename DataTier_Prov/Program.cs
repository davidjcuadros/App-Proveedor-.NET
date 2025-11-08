
using DataTier_Prov.Repositories;
using DataTier_Prov.Services;
using Microsoft.EntityFrameworkCore;
using ProveedorApp.Persistance;

namespace DataTier_Prov;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddDbContextFactory<MyAppDbContext>(
            options => options.UseNpgsql(connectionString)
        );

        // 2) registrar repos
        builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

        // 3) registrar gRPC SERVER
        builder.Services.AddGrpc();

        var app = builder.Build();

        // 4) mapear tu servicio grpc (el real, NO el greeter)
        app.MapGrpcService<ProductosServiceGrpcImpl>(); // <- esta clase todavía no la has creado

        // opcional
        app.MapGet("/", () => "Servidor GRPC arriba. Este endpoint no acepta REST.");

        app.Run();
    }
}
