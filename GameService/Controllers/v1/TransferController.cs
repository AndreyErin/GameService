using GameService.Models.Db.TransactionData;
using Microsoft.AspNetCore.Mvc;

namespace GameService.Controllers.v1
{
    [ApiController]
    [Route("/api/v1/transactions")]
    public class TransferController : Controller
    {
        private ITransactionsRepository _tRep;
        public TransferController(ITransactionsRepository transactionsRepository)
        {
            _tRep = transactionsRepository;
        }

        [HttpPost]
        public IActionResult SetTransaction(int bet, int payeeid, int senderid)
        {
            int result = _tRep.SetTransaction(bet, payeeid, senderid);

            switch (result) 
            {
                case 1:
                    return Ok();;
                case 0:
                    return BadRequest("Ошибка. Недостаточно средств.");
                case -2:
                    return BadRequest("Ошибка. Сбой при проведении транзакции.");
                default:
                    return BadRequest("Ошибка. Неверные id.");
            }
        }
    }
}
