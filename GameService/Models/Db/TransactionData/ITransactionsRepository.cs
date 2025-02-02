namespace GameService.Models.Db.TransactionData
{
    public interface ITransactionsRepository
    {
        int SetTransaction(decimal bet, int payeeid, int senderid, int? matchid = null);
    }
}
