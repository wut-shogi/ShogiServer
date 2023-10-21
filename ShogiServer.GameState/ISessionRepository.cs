using ShogiServer.GameState.Model;

namespace ShogiServer.GameState
{
    public interface ISessionRepository
    {
        Session? Get(Guid sessionId);
        Task<Session> Create(Player onBehalf);
    }
}