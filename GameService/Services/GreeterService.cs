using GameService;
using GameService.Models.Inst;
using GameService.Models.User;
using Grpc.Core;
using System.Text.Json;

namespace GameService.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private static List<User> usersOnline = new List<User>();

        private FakeUserRepository _users;
        private InstancesService _instances;
        public GreeterService(FakeUserRepository fakeUserRepository, InstancesService instancesService)
        {
            _users = fakeUserRepository;
            _instances = instancesService;
        }

        public override Task<LoginReply> Login(LoginRequest request, ServerCallContext context)
        {
            //находим пользователя
            User? user = _users.Get(request.Name);

            //защита от повторного входа
            if (usersOnline.FirstOrDefault(x => x.Id == user?.Id) != null) 
            {
                return Task.FromResult(new LoginReply { Id = -2 });
            }

            string password = "0000";
            //тут идет проверка существования пользователя, пароля)
            if (user != null && request.Password == password)
            {
                usersOnline.Add(user);
                return Task.FromResult(new LoginReply { Id = user.Id });
            }

            //неверный логин/пароль
            return Task.FromResult(new LoginReply { Id = -1 });
        }

        public override Task<BalanceReplay> GetBalance(BalanceRequest request, ServerCallContext context)
        {
            User? user = _users.Get(request.Id);
            if(user != null) 
            { 
                int money = user.Money;
                return Task.FromResult(new BalanceReplay() { Money = money });
            }
            return Task.FromResult(new BalanceReplay() { Money = -1});
        }

        public override Task<GetGamesReplay> GetGames(GetGamesRequest request, ServerCallContext context)
        {
            string jsonString = JsonSerializer.Serialize(_instances.instances);

            return Task.FromResult(new GetGamesReplay() { GamesList = jsonString});
        }
    }
}
