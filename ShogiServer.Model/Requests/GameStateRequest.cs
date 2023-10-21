namespace ShogiServer.Model.Requests
{
    public class GameStateRequest
    {
        public Guid SessionId { get; set; }
        public Guid PlayerId { get; set; }
        public string Token { get; set; } = null!;
    }
}
