using GameService.Models.Db;
using GameService.Models.Inst;
using Microsoft.AspNetCore.Mvc;

namespace GameService.Controllers
{
    [ApiController]
    [Route("/api")]
    public class TransferController : Controller
    {
        private InstancesService _instancesService;
        private AppDbContext _db;
        public TransferController(InstancesService instancesService, AppDbContext appDbContext)
        {
            _instancesService = instancesService;
            _db = appDbContext;
        }

        [HttpGet]
        public string Index()
        {
            string a = _instancesService.instances[1].Id.ToString() + " " + _instancesService.instances[1].Bet.ToString() + " руб.";
            a = _db.users.ToList().First().name.ToString();
            return a;
        }
    }
}
