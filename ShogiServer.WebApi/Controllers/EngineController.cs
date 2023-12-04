using Microsoft.AspNetCore.Mvc;

namespace ShogiServer.WebApi.Controllers
{
    [Route("engine")]
    [ApiController]
    public class EngineController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> GetBestMove(string boardState)
        {
            return Ok(boardState);
        }
    }
}
