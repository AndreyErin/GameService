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

        public async Task GoToGameAsync()
        {
            while (true) 
            {
                Console.WriteLine("Введите номер комнаты");

                int numberRoom = GetValidNumber();

                var result = await _client.ConnectToGameAsync(new ConnectToGameRequest() { IdGame = numberRoom, IdPlayer = _id });

                switch (result.CountPlayers)
                {
                    //ждем второго игрока
                    case 1:
                        Console.WriteLine("Вы присоединились к матчу. Ждем второго игрока!");
                        var start = await _client.WaitiningStartGameAsync(new WaitiningStartGameRequest() { IdGame = numberRoom});
                        Console.WriteLine("Все в сборе. Делайте свой ход!");
                        int battle = await Battle(numberRoom);
                        if (battle == _id)
                        {
                            Console.WriteLine("Победа!");
                        }
                        else if(battle == -1)
                        {
                            Console.WriteLine("Ничья.");
                        }
                        else
                        {
                            Console.WriteLine("Поражение((");
                        }

                        return;
                    //начинаем игру сразу
                    case 2:
                        Console.WriteLine("Все в сборе. Делайте свой ход!");
                        int battle1 = await Battle(numberRoom);
                        if (battle1 == _id)
                        {
                            Console.WriteLine("Победа!");
                        }
                        else if (battle1 == -1)
                        {
                            Console.WriteLine("Ничья.");
                        }
                        else
                        {
                            Console.WriteLine("Поражение((");
                        }

                        return;
                    default:
                        await FailConnectToGame();
                        break;
                }
            }
        }

        private int GetValidNumber()
        {
            bool validNumber = int.TryParse(Console.ReadLine(), out int numberRoom);

            while (!validNumber)
            {
                validNumber = int.TryParse(Console.ReadLine(), out int nR);
                numberRoom = nR;
            }

            return numberRoom;
        }

        private async Task FailConnectToGame()
        {
            Console.WriteLine("Не удалось подключиться к игре.");
            Console.WriteLine("Актуальный список комнат:");

            List<Instance> instList = await GetGamesAsync();

            if (instList.Count == 0)
            {
                Console.WriteLine("Нет доступных комнат.");
                return;
            }

            foreach (Instance item in instList)
            {
                int countPlayers = 0;
                if (item.FirstPlayerId != 0)
                {
                    countPlayers = 1;
                }
                Console.WriteLine($"Комната №{item.Id}, ставка: {item.Bet}руб. игроков: {countPlayers}");
            }
        }

        private async Task<int> Battle(int room)
        {
            string key = Console.ReadLine().ToUpper();
            while (key != "K" && key != "N" && key != "B") 
            {
                key = Console.ReadLine();
            }
            Console.WriteLine("Ваш ход :" + key);

            var result = await _client.GetResultBattleAsync(new ResultBattleRequest() { IdGame = room, IdPlayer = _id, Key = key });
            return result.Winner;
        }
    }
}
