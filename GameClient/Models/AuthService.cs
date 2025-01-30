namespace GameClient.Models
{
    public class AuthService : IAuthService
    {
        //авторизация
        public async Task<int> AuthorizeAsync(Greeter.GreeterClient client)
        {          
            while (true)
            {
                Console.WriteLine("Введите логин");
                string name = Console.ReadLine();

                Console.WriteLine("Введите пароль");
                string password = Console.ReadLine();

                //обмен сообщениями
                var result = await client.LoginAsync(new LoginRequest { Name = name, Password = password });

                switch (result.Id)
                {
                    case > 0:
                        Console.WriteLine($"Привет, {name}!");
                        return result.Id;
                    case -1:
                        Console.WriteLine("Ошибка! Неверный логин или пароль.");
                        break;
                    case -2:
                        Console.WriteLine("Ошибка! Тользователь с такими данными уже в онлайне. Повторный вход не возможен.");
                        break;
                }

            }
        }
    }
}
