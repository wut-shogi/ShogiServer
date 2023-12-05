namespace ShogiServer.WebApi.Model
{
    public class Invitation
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Player InvitingPlayer { get; set; } = null!;
        public Player InvitedPlayer { get; set; } = null!;
    }
}
