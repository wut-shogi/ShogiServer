using SignalRSwaggerGen.Attributes;

namespace ShogiServer.WebApi.Hubs
{
    [SignalRHub]
    public interface IGameClient
    {
        Task SendGameState(Game game);
        Task SendGameConclusion(Game game);
    }
}