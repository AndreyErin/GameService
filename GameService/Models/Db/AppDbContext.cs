using GameService.Models.Db.TransactionData;
using GameService.Models.Db.UserData;
using Microsoft.EntityFrameworkCore;

namespace GameService.Models.Db
{
    public class AppDbContext: DbContext
    {
        public DbSet<User> users { get; set; }
        public DbSet<MatchHistory> matchhistories { get; set; }
        public DbSet<GameTransaction> gametransactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=gameservice;Username=postgres;Password=2007737");
        }
    }
}
