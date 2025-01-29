using GameService.Models.Inst;
using Microsoft.AspNetCore.Mvc;

namespace GameService.Controllers
{
    [ApiController]
    [Route("/api")]
    public class TransferController : Controller
    {
        private InstancesService _instancesService;
        public TransferController(InstancesService instancesService)
        {
            _instancesService = instancesService;
        }

        [HttpGet]
        public string Index()
        {
            string a = _instancesService.instances[1].Id.ToString() + " " + _instancesService.instances[1].Bet.ToString() + " руб.";

            return a;
        }
    }
}
