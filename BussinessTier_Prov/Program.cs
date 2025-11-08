
namespace BussinessTier_Prov;

using BussinessTier_Prov.Business;
using BussinessTier_Prov.IBusiness;
using Grpc.Net.ClientFactory;


using Productos;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var dataTierUrl = builder.Configuration["DataTierUrl"];
        
        builder.Services.AddScoped<IProductoBusiness, ProductoBusiness>();

        builder.Services.AddGrpcClient<ProductosService.ProductosServiceClient>(o =>
        {
            o.Address = new Uri(dataTierUrl);
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        



        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
