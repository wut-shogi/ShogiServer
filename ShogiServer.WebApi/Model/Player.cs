namespace ShogiServer.WebApi.Model
{
    public class Player
    {
        public string Nickname { get; set; } = null!;

        public string ConnectionId { get; set; } = Guid.NewGuid().ToString();

        public PlayerState State { get; set; } = PlayerState.Ready;
        
    }

    public enum PlayerState
    {
        Ready, Inviting, Invited
    }

    public class AuthenticatedPlayer : Player
    {
        public string Token { get; set; } = Guid.NewGuid().ToString();

        public Player ToPlayer()
        {
            return new Player { Nickname = Nickname, ConnectionId = ConnectionId };
        }
    }
}
