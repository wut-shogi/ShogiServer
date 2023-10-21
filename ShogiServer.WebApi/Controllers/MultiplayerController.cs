using Microsoft.AspNetCore.Mvc;
using ShogiServer.Model.Requests;
using ShogiServer.Model.Responses;

namespace ShogiServer.WebApi.Controllers
{
    [Route("multiplayer")]
    [ApiController]
    public class MultiplayerController : ControllerBase
    {
        // POST: multiplayer/create
        [HttpPost("start")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StartMultiplayerResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public IActionResult Start([FromBody] StartMultiplayerRequest request)
        {
            return BadRequest("Not implemented yet.");
        }

        // POST multiplayer/makeMove
        [HttpPost("makeMove")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MakeMoveResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult MakeMove([FromBody] MakeMoveRequest request)
        {
            return BadRequest("Not implemented yet.");
        }

        // POST multiplayer/keepAlive
        [HttpPost("keepAlive")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult KeepAlive([FromBody] KeepAliveRequest request)
        {
            return BadRequest("Not implemented yet.");
        }

        // POST multiplayer/gameState
        [HttpPost("gameState")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GameStateResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GameState([FromBody] GameStateRequest request)
        {
            return BadRequest("Not implemented yet.");
        }
    }
}
