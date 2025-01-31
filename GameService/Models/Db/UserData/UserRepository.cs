using GameService.Models.Db;


namespace GameService.Models.Db.UserData
{
    public class UserRepository : IUserRepository
    {
        private AppDbContext _db;
        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        public User? Get(int id)
        {
            return _db.users.FirstOrDefault(x => x.id == id);
        }
        public User? Get(string name)
        {
            return _db.users.FirstOrDefault(x => x.name == name);
        }
        public List<User> GetAll()
        {
            return _db.users.ToList();
        }
    }
}
