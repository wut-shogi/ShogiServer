namespace ShogiServer.WebApi.Model
{
    public class Invitation
    {
        public Guid Id { get; set; }

        public Guid InvitingPlayerId { get; set; }
        public Player InvitingPlayer { get; set; } = null!;


        public Guid InvitedPlayerId { get; set; }
        public Player InvitedPlayer { get; set; } = null!;
    }
}
