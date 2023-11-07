using SignalRSwaggerGen.Attributes;

namespace ShogiServer.WebApi.Hubs
{
    [SignalRHub]
    public interface IGameHub
    {
        Task MakeMove(string move);
    }
}