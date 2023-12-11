using ShogiServer.WebApi.Model;
using SignalRSwaggerGen.Attributes;

namespace ShogiServer.WebApi.Hubs
{
    [SignalRHub]
    public interface IMatchmakingClient
    {
        Task SendLobby(List<Player> lobby);
        Task SendPlayer(Player createdPlayer);
        Task SendInvitation(Invitation invitation);
        Task SendRejection();
        Task SendCreatedGame(Game game);
    }

}
