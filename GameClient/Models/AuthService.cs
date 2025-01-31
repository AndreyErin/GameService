using Grpc.Net.Client;

namespace GameClient.Models
{
    public class AuthService : IAuthService
    {
        private string _url;
        private int _id = 0;
        public AuthService(string url)
        {
            _url = url;
        }

        public async Task<int> LoginAsync()
        {          
            while (true)
            {
                Console.WriteLine("Введите логин (подсказка: Maks, Kolia, Misha)");
                string name = Console.ReadLine();

                Console.WriteLine("Введите пароль (подсказка: 0000)");
                string password = Console.ReadLine();

                //канал для обмена сообщениями
                using var channel = GrpcChannel.ForAddress(_url);
                //клиент
                Greeter.GreeterClient client = new Greeter.GreeterClient(channel);
                
                //обмен сообщениями
                var result = await client.LoginAsync(new LoginRequest { Name = name, Password = password });

                switch (result.Id)
                {
                    case > 0:
                        Console.WriteLine($"Привет, {name}!");
                        _id = result.Id;
                        return _id;
                    case -1:
                        Console.WriteLine("Ошибка! Неверный логин или пароль.");
                        break;
                    case -2:
                        Console.WriteLine("Ошибка! Тользователь с такими данными уже в онлайне. Повторный вход не возможен.");
                        break;
                }

            }
        }

        public async Task<int> LogoutAsync()
        {
            //канал для обмена сообщениями
            using var channel = GrpcChannel.ForAddress(_url);
            //клиент
            Greeter.GreeterClient client = new Greeter.GreeterClient(channel);

            await client.LogoutAsync(new LogoutRequest() { Id = _id});
            return 0;
        }
    }
}
