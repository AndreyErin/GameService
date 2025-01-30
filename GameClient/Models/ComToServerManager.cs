using Grpc.Net.Client;
using System.Text.Json;

namespace GameClient.Models
{
    public class ComToServerManager
    {
        private int _id;
        private Greeter.GreeterClient _client;
        private IAuthService _authService;

        public ComToServerManager(string url, IAuthService authService) 
        {
            _authService = authService;

            //канал для обмена сообщениями
            var channel = GrpcChannel.ForAddress(url);
            //клиент
            _client = new Greeter.GreeterClient(channel);           
        }

        public async Task Authorize()
        {
            _id = await _authService.AuthorizeAsync(_client);
        }

        public async Task<int> GetBalanceAsync()
        {
            var result = await _client.GetBalanceAsync(new BalanceRequest() { Id = _id });
            return result.Money;
        }

        public async Task<List<Instance>> GetGamesAsync()
        {
            var result = await _client.GetGamesAsync(new GetGamesRequest());
            List<Instance>? instances = JsonSerializer.Deserialize<List<Instance>>(result.GamesList);

            if (instances != null)
            {
                return instances;
            }

            return new();
        }
    }
}
