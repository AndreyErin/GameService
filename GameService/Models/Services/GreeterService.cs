using GameService.Models.Db.UserData;
using GameService.Models.Inst;
using Grpc.Core;
using System.Text.Json;

namespace GameService.Models.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private static List<User> usersOnline = new List<User>();

        private IUserRepository _users;
        private InstancesService _instances;
        public GreeterService(IUserRepository userRepository, InstancesService instancesService)
        {
            _users = userRepository;
            _instances = instancesService;
        }

        public override Task<LoginReply> Login(LoginRequest request, ServerCallContext context)
        {
            //находим пользователя
            User? user = _users.Get(request.Name);

            //защита от повторного входа
            if (usersOnline.FirstOrDefault(x => x.id == user?.id) != null)
            {
                return Task.FromResult(new LoginReply { Id = -2 });
            }

            string password = "0000";
            //тут идет проверка существования пользователя, пароля)
            if (user != null && request.Password == password)
            {
                usersOnline.Add(user);
                return Task.FromResult(new LoginReply { Id = user.id });
            }

            //неверный логин/пароль
            return Task.FromResult(new LoginReply { Id = -1 });
        }

        public override Task<BalanceReply> GetBalance(BalanceRequest request, ServerCallContext context)
        {
            User? user = _users.Get(request.Id);
            if (user != null)
            {
                string money = user.balance.ToString();
                return Task.FromResult(new BalanceReply() { Money = money });
            }
            return Task.FromResult(new BalanceReply() { Money = "-1" });
        }

        public override Task<GetGamesReply> GetGames(GetGamesRequest request, ServerCallContext context)
        {
            var subInst = _instances.instances.Where(x => x.IsVisible == true);

            string jsonString = JsonSerializer.Serialize(subInst);

            return Task.FromResult(new GetGamesReply() { GamesList = jsonString });
        }

        public override Task<ConnectToGameReply> ConnectToGame(ConnectToGameRequest request, ServerCallContext context)
        {
            //если такой комнаты нет или все слоты заняты
            var room = _instances.instances.FirstOrDefault(x => x.Id == request.IdGame && x.IsVisible == true);
            if (room == null)
            {
                return Task.FromResult(new ConnectToGameReply() { CountPlayers = -1 });
            }

            bool connect = room.AddPlayer(request.IdPlayer);
            if (connect)
            {
                int countPlrs = room.SecondPlayerId == 0 ? 1 : 2;
                return Task.FromResult(new ConnectToGameReply() { CountPlayers = countPlrs });
            }
            else
            {
                return Task.FromResult(new ConnectToGameReply() { CountPlayers = -1 });
            }
        }

        public override Task<WaitiningStartGameReply> WaitiningStartGame(WaitiningStartGameRequest request, ServerCallContext context)
        {
            var room = _instances.instances.FirstOrDefault(x => x.Id == request.IdGame);
            //если что-то пошло не так
            if (room == null)
            {
                return Task.FromResult(new WaitiningStartGameReply() { Start = false });
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

            return Task.FromResult(new WaitiningStartGameReply() { Start = true });
        }

        public override async Task<ResultBattleReply> GetResultBattle(ResultBattleRequest request, ServerCallContext context)
        {
            var room = _instances.instances.FirstOrDefault(x => x.Id == request.IdGame);
            //если что-то пошло не так
            if (room == null)
            {
                return new ResultBattleReply() { Winner = 0 };
            }
            //делаем ход
            room.Battle(request.IdPlayer, request.Key);

            bool wait = true;
            //ждем результата
            while (wait)
            {
                await Task.Delay(1000);

                if (room?.Winner != 0)
                {
                    wait = false;
                }
            }

            //убераем комнату из списка
            _instances.instances.Remove(room);

            return new ResultBattleReply() { Winner = room.Winner };
        }

        public override Task<LogoutReply> Logout(LogoutRequest request, ServerCallContext context)
        {
            int id = request.Id;

            User? user = usersOnline.Find(x => x.id == id);

            if (user != null)
            {
                usersOnline.Remove(user);
            }
            
            return Task.FromResult(new LogoutReply());
        }
    }
}
