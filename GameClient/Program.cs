using GameClient.Models;
using Grpc.Net.Client;
using System.Text.Json;

namespace GameClient
{
    internal class Program
    {
        static string _name = "";
        static async Task Main(string[] args)
        {
            Console.WriteLine("Добро пожаловать в игру Камень-Ножницы-Бумага");

            //канал для обмена сообщениями
            using var channel = GrpcChannel.ForAddress("https://localhost:7089");

            //клиент
            var client = new Greeter.GreeterClient(channel);

            //авторизация
            bool success = false;
            while (success == false)
            {
                Console.WriteLine("Введите логин");
                string name = Console.ReadLine();

                Console.WriteLine("Введите пароль");
                string password = Console.ReadLine();

                //обмен сообщениями
                var result = await client.LoginAsync(new LoginRequest { Name = name, Password = password });

                success = result.IsLogin;
                if (success == false)
                {
                    Console.WriteLine("Ошибка! Неверный логин или пароль.");
                }
                else 
                {
                    _name = name;
                    Console.WriteLine($"Привет, {name}!");
                }
            }

            Console.WriteLine(@"Меню: \b - баланс, \g - доступные матчи, \c [номер-комнаты] - присоединиться к матчу, \v - закрыть приложение");
            //меню

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
                        var resultB = await client.GetBalanceAsync(new BalanceRequest() { Name = _name});
                        Console.WriteLine($"Ваш баланс: {resultB.Money}руб.");
                        break;
                    case "g":
                        var resultG = await client.GetGamesAsync(new GetGamesRequest());
                        List<Instance>? instances = JsonSerializer.Deserialize<List<Instance>>(resultG.GamesList);

                        foreach (var item in instances)
                        {
                            int countPlayers = 0;
                            if (item.FirstPlayerId != null)
                            {
                                countPlayers = 1;
                            }
                            
                            Console.WriteLine($"Комната №{item.Id}, ставка: {item.Bet}руб. игроков: {countPlayers}");
                        }
                        
                        break;
                    case "c":
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
    }
}
