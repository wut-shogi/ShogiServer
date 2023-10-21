using static ShogiServer.GameState.Model.Player;

namespace ShogiServer.Model.Responses
{
    public class StartMultiplayerResponse
    {
        public Guid SessionId { get; set; }
        public Guid PlayerId { get; set; }
        public PlayerColor PiecesColor { get; set; }
        public string Token { get; set; } = null!;
        public string OpponentName { get; set; } = null!;
    }
}
