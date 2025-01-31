namespace GameClient.Models
{
    public interface IAuthService
    {
        Task<int> LoginAsync();
        Task<int> LogoutAsync();
    }
}
