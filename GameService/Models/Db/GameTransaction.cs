namespace GameService.Models.Db
{
    public class GameTransaction
    {
        public int id { get; set; }
        public int matchnumber { get; set; }
        public int senderplayerid { get; set; }
        public int payeeplayerid { get; set; }
        public decimal bet { get; set; }
        public DateTime datematch { get; set; }
    }
}
