using GameClient.Models;

namespace GameClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Добро пожаловать в игру Камень-Ножницы-Бумага");

            AuthService authService = new AuthService();
            ComToServerManager serverManager = new("https://localhost:7089", authService);

            //авторизация
            await serverManager.Authorize();
            //выводим меню
            Console.WriteLine(@"Меню: \b - баланс, \g - доступные матчи, \c - присоединиться к матчу, \v - закрыть приложение");
            
            //взаимодействие с пользователем
            while (true)
            {
                string inputLine = Console.ReadLine();

                string slash = inputLine.Substring(0,1);

                if (slash != @"\")
                {
                    Console.WriteLine("Неверная команда");
                    continue;
                }

                string command = inputLine.Substring(1, 1);
                switch (command) 
                { 
                    case "b":
                        int balance = await serverManager.GetBalanceAsync();
                        Console.WriteLine($"Ваш баланс: {balance}руб.");
                        break;
                    case "g":
                        await GetGameList(serverManager);
                        break;
                    case "c":
                        await serverManager.GoToGameAsync();
                        //выводим меню
                        Console.WriteLine(@"Меню: \b - баланс, \g - доступные матчи, \c - присоединиться к матчу, \v - закрыть приложение");
                        break;
                    case "v":
                        //выход
                        return;
                        
                    default:
                        Console.WriteLine("Неверная команда");
                        break;
                }
            }
        }

        private static async Task GetGameList(ComToServerManager serverManager)
        {
            List<Instance> instances = await serverManager.GetGamesAsync();
            if (instances.Count == 0) 
            {
                Console.WriteLine("Нет доступных комнат.");
                return;
            }

            foreach (var item in instances)
            {
                int countPlayers = 0;
                if (item.FirstPlayerId != 0)
                {
                    countPlayers = 1;
                }

                Console.WriteLine($"Комната №{item.Id}, ставка: {item.Bet}руб. игроков: {countPlayers}");
            }
        }
    }
}
