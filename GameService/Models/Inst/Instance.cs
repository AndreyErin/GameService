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
        public int? FirstPlayerId { get; set; } = null;
        public int? SecondPlayerId { get; set; } = null;
        public bool IsVisible { get; set; } = true;

        public bool AddPlayer(int idPlayer)
        {
            if (FirstPlayerId == null)
            {
                FirstPlayerId = idPlayer;
                return true;
            }
            else if (SecondPlayerId == null)
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
    }
}
