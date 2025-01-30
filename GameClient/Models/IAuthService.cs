
namespace GameClient.Models
{
    public interface IAuthService
    {
        Task<int> AuthorizeAsync(Greeter.GreeterClient client);
    }
}
