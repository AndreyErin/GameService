using GameService.Models.Db;
using GameService.Models.Db.MatchHistoryData;
using GameService.Models.Db.TransactionData;
using GameService.Models.Db.UserData;
using GameService.Models.Inst;
using GameService.Models.Services;
using Microsoft.EntityFrameworkCore;

namespace GameService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();

            //ад
            string ConnectionString = builder.Configuration.GetConnectionString("Default");
            builder.Services.AddDbContext<AppDbContext>(opt=> opt.UseNpgsql(ConnectionString));

            builder.Services.AddSingleton<InstancesService>();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ITransactionsRepository, TransactionsRepository>();
            builder.Services.AddScoped<IMacthHistoryRepository, MacthHistoryRepository>();

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