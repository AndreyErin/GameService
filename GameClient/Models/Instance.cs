namespace GameClient.Models
{
    //Экземляр матча
    public class Instance
    {
        public int Id { get; set; }
        public decimal Bet { get; set; }
        public int FirstPlayerId { get; set; }
        public int SecondPlayerId { get; set; }
        public bool IsVisible { get; set; } = true;
        public int Winner { get; set; }

    }
}
