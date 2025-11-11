using BussinessTier_Prov.Business;
using BussinessTier_Prov.IBusiness;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Grpc.Net.ClientFactory;
using Productos;

namespace BussinessTier_Prov
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ============================================
            // API REST
            // ============================================
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ============================================
            // Capa de negocio
            // ============================================
            builder.Services.AddScoped<IProductoBusiness, ProductoBusiness>();

            // ============================================
            // VALIDAR URL DEL DATATIER 
            // ============================================
            var dataTierUrl = builder.Configuration["Services:DataTierUrl"]
                ?? throw new InvalidOperationException(
                    "La URL del DataTier no está configurada en appsettings.json (clave: Services:DataTierUrl)."
                );

            // ============================================
            // Cliente gRPC → DataTier
            // ============================================
            builder.Services.AddGrpcClient<ProductosService.ProductosServiceClient>(o =>
            {
                o.Address = new Uri(dataTierUrl);
            });

            var app = builder.Build();

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
}
