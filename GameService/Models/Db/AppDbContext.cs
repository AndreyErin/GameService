using GameService.Models.Db.MatchHistoryData;
using GameService.Models.Db.TransactionData;
using GameService.Models.Db.UserData;
using Microsoft.EntityFrameworkCore;

namespace GameService.Models.Db
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }

        public DbSet<User> users { get; set; }
        public DbSet<MatchHistory> matchhistories { get; set; }
        public DbSet<GameTransaction> gametransactions { get; set; }
    }
}
