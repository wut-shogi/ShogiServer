namespace ShogiServer.Model.Requests
{
    public class KeepAliveRequest
    {
        public Guid SessionId { get; set; }
        public Guid PlayerId { get; set; }
        public string Token { get; set; } = null!;
    }
}
