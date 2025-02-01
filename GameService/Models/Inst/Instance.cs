using GameService.Models.Db;
using GameService.Models.Db.TransactionData;
using GameService.Models.Db.UserData;

namespace GameService.Models.Inst
{
    //Экземляр матча
    public class Instance
    {
        //счетчик Id
        private static ushort _newId = 0;
        public Instance(int bet)
        {           
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

        private int _winner;
        public int Winner {
            get { return _winner; }
            set 
            {
                _winner = value;
                Task.Run(SaveResult); 
            }  
        }

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

            if (_firstPlayerMove != null && _secondPlayerMove != null)
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
            }            
        }

        private void SaveResult()
        {
            var db = new AppDbContext();

            MatchHistory matchHistory = new MatchHistory() 
            {  
                bet = Bet, 
                firstplayerid = FirstPlayerId, 
                secondplayerid = SecondPlayerId,
                firstplayerkey = _firstPlayerMove,
                secondplayerkey = _secondPlayerMove,
                datematch = DateTime.UtcNow,
                winner = Winner != -1? Winner : null,
            };
            db.matchhistories.Add(matchHistory);
            db.SaveChanges();
          
            //если есть победитель, то проводим транзакцию
            if (Winner != -1) 
            {
                int matchId = matchHistory.id;
                SetTransaction(db, matchId);
            }
            else
            {
                db.Dispose();
            }
        }

        private void SetTransaction(AppDbContext db, int matchId) 
        {
            //тот, кто платит
            int senderPlayerId = FirstPlayerId == Winner ? SecondPlayerId : FirstPlayerId;
            bool transactionSuccess = true;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    User user = db.users.First(x => x.id == senderPlayerId);
                    user.balance -= Bet;
                    db.SaveChanges();

                    User userWin = db.users.First(x => x.id == Winner);
                    userWin.balance += Bet;
                    db.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception)
                {
                    //отменяем
                    transaction.Rollback();
                    transactionSuccess = false;
                }
            }

            if (transactionSuccess) 
            {
                GameTransaction gameTransaction = new GameTransaction()
                {
                    bet = Bet,
                    datematch = DateTime.UtcNow,
                    matchid = matchId,
                    payeeplayerid = Winner,
                    senderplayerid = senderPlayerId
                };

                db.gametransactions.Add(gameTransaction);
                db.SaveChanges();
            }

            db.Dispose();
        }
    }
}
