using GameService.Models.Inst;
using GameService.Models.UserData;
using Grpc.Core;
using System.Text.Json;

namespace GameService.Models.Services
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
            //������� ������������
            User? user = _users.Get(request.Name);

            //������ �� ���������� �����
            if (usersOnline.FirstOrDefault(x => x.Id == user?.Id) != null)
            {
                return Task.FromResult(new LoginReply { Id = -2 });
            }

            string password = "0000";
            //��� ���� �������� ������������� ������������, ������)
            if (user != null && request.Password == password)
            {
                usersOnline.Add(user);
                return Task.FromResult(new LoginReply { Id = user.Id });
            }

            //�������� �����/������
            return Task.FromResult(new LoginReply { Id = -1 });
        }

        public override Task<BalanceReply> GetBalance(BalanceRequest request, ServerCallContext context)
        {
            User? user = _users.Get(request.Id);
            if (user != null)
            {
                string money = user.Money.ToString();
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
            //���� ����� ������� ��� ��� ��� ����� ������
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
            //���� ���-�� ����� �� ���
            if (room == null)
            {
                return Task.FromResult(new WaitiningStartGameReply() { Start = false });
            }

            bool wait = true;
            //���� ��� ������ ������������ � ����
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


        public override Task<ResultBattleReply> GetResultBattle(ResultBattleRequest request, ServerCallContext context)
        {
            var room = _instances.instances.FirstOrDefault(x => x.Id == request.IdGame);
            //���� ���-�� ����� �� ���
            if (room == null)
            {
                return Task.FromResult(new ResultBattleReply() { Winner = 0 });
            }
            //������ ���
            room.Battle(request.IdPlayer, request.Key);

            bool wait = true;
            //���� ����������
            while (wait)
            {
                Thread.Sleep(1000);

                if (room?.Winner != 0)
                {
                    wait = false;
                }
            }

            return Task.FromResult(new ResultBattleReply() { Winner = room.Winner });
        }

        public override Task<LogoutReply> Logout(LogoutRequest request, ServerCallContext context)
        {
            int id = request.Id;

            User? user = usersOnline.Find(x => x.Id == id);

            if (user != null)
            {
                usersOnline.Remove(user);
            }
            
            return Task.FromResult(new LogoutReply());
        }
    }
}
