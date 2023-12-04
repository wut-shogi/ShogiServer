using SignalRSwaggerGen.Attributes;

namespace ShogiServer.WebApi.Hubs
{
    [SignalRHub]
    public interface IGameClient
    {
        Task SendGameState(Game game);
        Task SendGameConclusion(GameConclusion gameConclusion);
    }

    public class GameConclusion
    {
        public Game Game { get; set; }
        public Game.PieceColor Winner { get; set; }
    }
}