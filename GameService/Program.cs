using GameService.Models.Db;
using GameService.Models.Db.TransactionData;
using GameService.Models.Db.UserData;
using GameService.Models.Inst;
using GameService.Models.Services;

namespace GameService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddSingleton<InstancesService>();

            //ад
            builder.Services.AddDbContext<AppDbContext>();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ITransactionsRepository, TransactionsRepository>();

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