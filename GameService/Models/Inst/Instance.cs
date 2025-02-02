using GameService.Models.Db.MatchHistoryData;
using GameService.Models.Db.TransactionData;

namespace GameService.Models.Inst
{
    //Экземляр матча
    public class Instance
    {

        private IServiceScopeFactory _scopeFactory;
        //счетчик Id
        private static ushort _newId = 0;
        public Instance(int bet, IServiceScopeFactory scopeFactory)
        {     
            _scopeFactory = scopeFactory;
            Bet = bet;

            _newId++;
            if (_newId > 65_000)
            {
                _newId = 1;
            }

            Id = _newId;
        }
       
        public int Id { get; set; }
        public decimal Bet { get; set; }
        public int FirstPlayerId { get; set; }     
        public int SecondPlayerId { get; set; }
        public bool IsVisible { get; set; } = true;

        private string _firstPlayerMove = "";
        private string _secondPlayerMove = "";
        public int Winner {  get; set; }

        public bool AddPlayer(int idPlayer)
        {
            if (FirstPlayerId == 0)
            {
                FirstPlayerId = idPlayer;
                return true;
            }
            else if (SecondPlayerId == 0)
            {
                SecondPlayerId = idPlayer;
                IsVisible = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Battle(int idPlayer, string move)
        {
            if (idPlayer == FirstPlayerId)
            {
                _firstPlayerMove = move;
            }
            else
            {
                _secondPlayerMove = move;
            }

            if (_firstPlayerMove != "" && _secondPlayerMove != "")
            {
                switch (_firstPlayerMove)
                {
                    case "К":
                        switch (_secondPlayerMove)
                        {
                            case "К":
                                //нечья
                                Winner = -1;
                                break;
                            case "Н":
                                Winner = FirstPlayerId;
                                break;
                            case "Б":
                                Winner = SecondPlayerId;
                                break;
                        }
                        break;
                    case "Н":
                        switch (_secondPlayerMove)
                        {
                            case "К":
                                Winner = SecondPlayerId;
                                break;
                            case "Н":
                                //нечья
                                Winner = -1;
                                break;
                            case "Б":
                                Winner = FirstPlayerId;
                                break;
                        }
                        break;
                    case "Б":
                        switch (_secondPlayerMove)
                        {
                            case "К":
                                Winner = FirstPlayerId;
                                break;
                            case "Н":
                                Winner = SecondPlayerId;
                                break;
                            case "Б":
                                //нечья
                                Winner = -1;
                                break;
                        }
                        break;
                }


                Task.Run(SaveResult);
            }            
        }

        private void SaveResult()
        {
            MatchHistory matchHistory = new MatchHistory()
            {
                bet = Bet,
                firstplayerid = FirstPlayerId,
                secondplayerid = SecondPlayerId,
                firstplayerkey = _firstPlayerMove,
                secondplayerkey = _secondPlayerMove,
                datematch = DateTime.UtcNow,
                winner = Winner != -1 ? Winner : null,
            };

            using (var scope = _scopeFactory.CreateScope())
            {
                var mhRepository = scope.ServiceProvider.GetService<IMacthHistoryRepository>();

                mhRepository?.Add(matchHistory);
            }

            //если есть победитель, то проводим транзакцию
            if (Winner != -1) 
            {
                int matchId = matchHistory.id;
                SetTransaction(matchId);
            }
        }

        private async void SetTransaction(int matchId) 
        {
            //тот, кто платит
            int senderPlayerId = FirstPlayerId == Winner ? SecondPlayerId : FirstPlayerId;

            using (var scope = _scopeFactory.CreateScope())
            {
                var tRepository = scope.ServiceProvider.GetService<ITransactionsRepository>();
                if (tRepository != null) 
                {
                    //3 попытки провести транзакцию
                    for (int i = 0; i < 3; i++)
                    {
                        int tResult = tRepository.SetTransaction(Bet, Winner, senderPlayerId, matchId);

                        if (tResult == 1)
                        {
                            return;
                        }

                        await Task.Delay(1000);
                    }
                }
            }    
        }
    }
}
