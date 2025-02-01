namespace GameService.Models.Db.TransactionData
{
    public interface ITransactionsRepository
    {
        int SetTransaction(int bet, int payeeid, int senderid);
    }
}
