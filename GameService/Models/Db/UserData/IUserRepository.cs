namespace GameService.Models.Db.UserData
{
    public interface IUserRepository
    {
        List<User> GetAll();
        User? Get(int id);
        User? Get(string name);
    }
}
