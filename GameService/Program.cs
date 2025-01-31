using GameService.Models.Inst;
using GameService.Models.Services;
using GameService.Models.UserData;


namespace GameService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddSingleton<InstancesService>();
            builder.Services.AddScoped<FakeUserRepository>();



            // Add services to the container.
            builder.Services.AddGrpc();

            var app = builder.Build();
            
            
            app.MapDefaultControllerRoute();
            // Configure the HTTP request pipeline.
            app.MapGrpcService<GreeterService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}