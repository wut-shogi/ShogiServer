using ShogiServer.WebApi.Model;
using SignalRSwaggerGen.Attributes;

namespace ShogiServer.WebApi.Hubs
{
    [SignalRHub]
    public interface IShogiClient
    {
        Task SendLobby(List<PlayerDTO> lobby);
        Task SendPlayer(Player createdPlayer);
        Task SendInvitation(InvitationDTO invitation);
        Task SendRejection();
        Task SendCreatedGame(GameDTO game);
        Task SendGameState(GameDTO game);
    }

    public class PlayerDTO
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; } = null!;
        public PlayerState State { get; set; }

        public static PlayerDTO FromDatabasePlayer(Player player)
        {
            return new PlayerDTO
            {
                Id = player.Id,
                Nickname = player.Nickname,
                State = player.State
            };
        }
    }

    public class InvitationDTO
    {
        public Guid Id { get; set; }
        public PlayerDTO InvitedPlayer { get; set; } = null!;
        public PlayerDTO InvitingPlayer { get; set; } = null!;

        public static InvitationDTO FromDatabaseInvitation(Invitation invitation)
        {
            return new InvitationDTO
            {
                Id = invitation.Id,
                InvitedPlayer = PlayerDTO.FromDatabasePlayer(invitation.InvitedPlayer),
                InvitingPlayer = PlayerDTO.FromDatabasePlayer(invitation.InvitingPlayer)
            };
        }
    }

    public class GameDTO
    {
        public Guid Id { get; set; }
        public PlayerDTO? BlackPlayer { get; set; } = null!;
        public PlayerDTO? WhitePlayer { get; set; } = null!;
        public string BoardState { get; set; } = null!;

        public static GameDTO FromDatabaseGame(Game game)
        {
            return new GameDTO
            {
                Id = game.Id,
                BlackPlayer = game.Black != null ? PlayerDTO.FromDatabasePlayer(game.Black!) : null,
                WhitePlayer = game.White != null ? PlayerDTO.FromDatabasePlayer(game.White!) : null,
                BoardState = game.BoardState
            };
        }
    }
}
