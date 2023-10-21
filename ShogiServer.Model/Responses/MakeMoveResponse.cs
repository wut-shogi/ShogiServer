using static ShogiServer.GameState.Model.GameState;

namespace ShogiServer.Model.Responses
{
    public class MakeMoveResponse
    {
        public string UsiGameState { get; set; } = null!;
        public GameStatus gameStatus { get; set; }
    }
}
