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
    }
}
