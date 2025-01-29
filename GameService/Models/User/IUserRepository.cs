namespace GameService.Models.User
{
    public interface IUserRepository
    {
        List<User> GetAll();
        void Add(User user);       
        User? Get(int id);
        User? Get(string name);
        int Update(User user);
        int Delete(int id);
    }
}
