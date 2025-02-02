namespace GameService.Models.Db.MatchHistoryData
{
    public class MacthHistoryRepository : IMacthHistoryRepository
    {
        private AppDbContext _db;
        public MacthHistoryRepository(AppDbContext appDbContext) 
        { 
            _db = appDbContext;
        }
        public int Add(MatchHistory matchHistory)
        {
            _db.Add(matchHistory);
            _db.SaveChanges();

            return matchHistory.id;
        }
    }
}
