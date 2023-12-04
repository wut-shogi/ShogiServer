using Microsoft.AspNetCore.Http.HttpResults;
using ShogiServer.WebApi.Model;
using SignalRSwaggerGen.Attributes;

namespace ShogiServer.WebApi.Hubs
{
    [SignalRHub]
    public interface IMatchmakingClient
    {
        Task SendLobby(List<Player> lobby);
        Task SendAuthenticatedPlayer(AuthenticatedPlayer createdPlayer);
        Task SendInvitation(Invitation invitation);
        Task SendRejection();
        Task SendCreatedGame(Game game);
    }

    public record JoinLobbyRequest(string Nickname);
    public record JoinLobbyResponse(Player Player);

    public record InviteRequest(Guid InvitingPlayerId, Guid InvitedPlayerId);
    public record InviteResponse(Invitation Invitation);

    public class Invitation
    {
        public Guid Id { get; set; }
        public Player InvitingPlayer { get; set; }
        public Player InvitedPlayer { get; set; }
    }

    public record AcceptInvitationRequest(Guid InvitationId);
    public record AcceptInvitationResponse(Game Game);

    public class Game
    {
        public Guid Id { get; set; }
        public Player WhitePlayer { get; set; }
        public Player BlackPlayer { get; set; }
        public Board Board { get; set; }
        public PieceColor Turn { get; set; }

        public enum PieceColor { Black, White };
    }

    public class Board
    {
        public string UsiBoardState { get; set; }
    }

    public record RejectInvitationRequest(Guid InvitationId);
    public record RejectInvitationResponse();
}
