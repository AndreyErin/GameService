
namespace GameService.Models.UserData
{
    public class FakeUserRepository : IUserRepository
    {
        private List<User> _users = new List<User>();
        public FakeUserRepository()
        {
            _users = new List<User>() { 
                new User { Id = 1, Name = "Maks", Money = 1111 },
                new User { Id = 2, Name = "Kolia", Money = 2222 },
                new User { Id = 3, Name = "Misha", Money = 3333 }
                };
        }

        public void Add(User user)
        {
            throw new NotImplementedException();
        }

        public int Delete(int id)
        {
            throw new NotImplementedException();
        }

        public User? Get(int id)
        {
            return _users.FirstOrDefault(x => x.Id == id);
        }

        public User? Get(string name)
        {
            return _users.FirstOrDefault(x => x.Name == name);
        }

        public List<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public int Update(User user)
        {
            throw new NotImplementedException();
        }
    }
}
