namespace ShogiServer.WebApi.Model
{
    public class Player
    {
        public string Nickname { get; set; } = null!;
    }

    public class AuthenticatedPlayer : Player
    {
        public string Token { get; set; } = Guid.NewGuid().ToString();
        public string ConnectionId { get; set; } = Guid.NewGuid().ToString();

        public Player ToPlayer()
        {
            return new Player { Nickname = Nickname };
        }
    }
}