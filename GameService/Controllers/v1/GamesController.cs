using GameService.Models.Inst;
using Microsoft.AspNetCore.Mvc;

namespace GameService.Controllers.v1
{
    [ApiController]
    [Route("/api/v1/games")]
    public class GamesController : Controller
    {
        private InstancesService _instancesService;
        public GamesController(InstancesService instancesService)
        {
            _instancesService = instancesService;
        }

        [HttpPost]
        public IActionResult AddGame(int bet)
        {
            int roomId = _instancesService.Add(bet);

            return Ok($"Комната создана. №{roomId}");
        }
    }
}
