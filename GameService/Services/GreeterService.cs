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
            var subInst = _instances.instances.Where(x=>x.IsVisible == true);

            string jsonString = JsonSerializer.Serialize(subInst);

            return Task.FromResult(new GetGamesReplay() { GamesList = jsonString});
        }

        public override Task<ConnectToGameReplay> ConnectToGame(ConnectToGameRequest request, ServerCallContext context)
        {
            //если такой комнаты нет или все слоты заняты
            var room = _instances.instances.FirstOrDefault(x=>x.Id == request.IdGame && x.IsVisible == true);
            if (room == null)
            {
                return Task.FromResult(new ConnectToGameReplay() { CountPlayers = -1 });
            }

            bool connect = room.AddPlayer(request.IdPlayer);
            if (connect) 
            {
                int countPlrs = room.SecondPlayerId == 0 ? 1 : 2;
                return Task.FromResult(new ConnectToGameReplay() { CountPlayers = countPlrs });
            }
            else
            {
                return Task.FromResult(new ConnectToGameReplay() { CountPlayers = -1 });
            }           
        }

        public override Task<WaitiningStartGameReplay> WaitiningStartGame(WaitiningStartGameRequest request, ServerCallContext context)
        {
            var room = _instances.instances.FirstOrDefault(x => x.Id == request.IdGame);
            //если что-то пошло не так
            if (room == null)
            {
                return Task.FromResult(new WaitiningStartGameReplay() { Start = false });
            }

            bool wait = true;
            //если оба игрока подключились к игре
            while (wait) 
            {
                Thread.Sleep(1000);
                
                if (room?.IsVisible == false)
                {
                    wait = false;
                }
            }

            return Task.FromResult(new WaitiningStartGameReplay() { Start = true });          
        }


        public override Task<ResultBattleReplay> GetResultBattle(ResultBattleRequest request, ServerCallContext context)
        {
            var room = _instances.instances.FirstOrDefault(x => x.Id == request.IdGame);
            //если что-то пошло не так
            if (room == null)
            {
                return Task.FromResult(new ResultBattleReplay() { Winner = 0 });
            }
            //делаем ход
            room.Battle(request.IdPlayer, request.Key);

            bool wait = true;
            //ждем результата
            while (wait)
            {
                Thread.Sleep(1000);

                if (room?.Winner != 0)
                {
                    wait = false;
                }
            }

            return Task.FromResult(new ResultBattleReplay() { Winner = room.Winner });
        }
    }
}
