namespace GameService.Models.Db.TransactionData
{
    public class GameTransaction
    {
        public int id { get; set; }
        public int? matchid { get; set; } = 0;
        public int senderplayerid { get; set; }
        public int payeeplayerid { get; set; }
        public decimal bet { get; set; }
        public DateTime datematch { get; set; }
    }
}
