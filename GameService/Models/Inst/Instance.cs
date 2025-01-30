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
        public int Bet { get; set; }
        public int FirstPlayerId { get; set; }
        public int SecondPlayerId { get; set; }
        public bool IsVisible { get; set; } = true;

        private string _firstPlayerMove = "";
        private string _secondPlayerMove = "";
        public int Winner { get; set; }

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
                    case "K":
                        switch (_secondPlayerMove)
                        {
                            case "K":
                                //нечья
                                Winner = -1;
                                break;
                            case "N":
                                Winner = FirstPlayerId;
                                break;
                            case "B":
                                Winner = SecondPlayerId;
                                break;
                        }
                        break;
                    case "N":
                        switch (_secondPlayerMove)
                        {
                            case "K":
                                Winner = SecondPlayerId;
                                break;
                            case "N":
                                //нечья
                                Winner = -1;
                                break;
                            case "B":
                                Winner = FirstPlayerId;
                                break;
                        }
                        break;
                    case "B":
                        switch (_secondPlayerMove)
                        {
                            case "K":
                                Winner = FirstPlayerId;
                                break;
                            case "N":
                                Winner = SecondPlayerId;
                                break;
                            case "B":
                                //нечья
                                Winner = -1;
                                break;
                        }
                        break;
                }
            }
        }
    }
}
