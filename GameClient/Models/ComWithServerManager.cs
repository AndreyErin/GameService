using Grpc.Net.Client;
using System.Text.Json;

namespace GameClient.Models
{
    public class ComWithServerManager
    {
        private int _id;
        private GrpcChannel _channel;
        private Greeter.GreeterClient _client;

        public ComWithServerManager(string url, int userId) 
        {
            _id = userId;

            //канал для обмена сообщениями
            _channel = GrpcChannel.ForAddress(url);
            //клиент
            _client = new Greeter.GreeterClient(_channel);           
        }

        public async Task<string> GetBalanceAsync()
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
                        
                        int winerId = await Battle(numberRoom);
                        Console.WriteLine(ShowBattleResult(winerId));
                        return;
                    //начинаем игру сразу
                    case 2:
                        int winerId2 = await Battle(numberRoom);
                        Console.WriteLine(ShowBattleResult(winerId2));
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
            Console.WriteLine("Все в сборе. Делайте свой ход!");

            string key = Console.ReadLine().ToUpper();
            while (key != "К" && key != "Н" && key != "Б") 
            {
                Console.WriteLine("Неверная команда. Введите К, Н или Б");
                key = Console.ReadLine();
            }
            Console.WriteLine($"Ваш ход :{key}. Ожидание хода соперника."  );

            var result = await _client.GetResultBattleAsync(new ResultBattleRequest() { IdGame = room, IdPlayer = _id, Key = key });
            return result.Winner;
        }

        private string ShowBattleResult(int winnerId)
        {
            string result = "";
            if (winnerId == _id)
            {
                result = "Победа!";
            }
            else if (winnerId == -1)
            {
                result = "Ничья.";
            }
            else
            {
                result = "Поражение((";
            }

            return result;
        }
        public void Dispose()
        {
            _channel.Dispose();
        }
    }
}
