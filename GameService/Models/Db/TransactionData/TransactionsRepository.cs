using GameService.Models.Db.UserData;

namespace GameService.Models.Db.TransactionData
{
    public class TransactionsRepository : ITransactionsRepository
    {
        private AppDbContext _db;
        public TransactionsRepository(AppDbContext db)
        {
            _db = db;
        }
        public int SetTransaction(int bet, int payeeid, int senderid)
        {
            User? payeePlayer = _db.users.FirstOrDefault(x=>x.id == payeeid);
            User? senderPlayer = _db.users.FirstOrDefault(x=>x.id==senderid);

            //неверные id или нет столько денег для перевода
            if (payeePlayer == null || senderPlayer == null)
            { 
                return -1;
            }

            if(senderPlayer.balance < bet)
            {
                return 0;
            }

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    payeePlayer.balance += bet;
                    _db.SaveChanges();

                    senderPlayer.balance -= bet;
                    _db.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception)
                {
                    //отменяем
                    transaction.Rollback();
                    return 1;
                }
            }

            GameTransaction gameTransaction = new GameTransaction()
            {
                bet = bet,
                datematch = DateTime.UtcNow,
                matchid = null,
                payeeplayerid = payeeid,
                senderplayerid = senderid
            };

            _db.gametransactions.Add(gameTransaction);
            _db.SaveChanges();

            return 1;
        }       
    }
}
