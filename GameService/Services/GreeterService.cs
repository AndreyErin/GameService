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

            string password = "0000";
            //тут идет проверка существования пользователя, пароля и защита от повторного входа)
            if (user != null && request.Password == password && (usersOnline.FirstOrDefault(x=>x.Id == user?.Id) == null))
            {
                usersOnline.Add(user);
                return Task.FromResult(new LoginReply { IsLogin = true });
            }

            //вход не удался
            return Task.FromResult(new LoginReply { IsLogin = false });
        }

        public override Task<BalanceReplay> GetBalance(BalanceRequest request, ServerCallContext context)
        {
            User? user = _users.Get(request.Name);
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
