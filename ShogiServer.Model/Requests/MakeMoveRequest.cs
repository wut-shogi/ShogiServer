namespace ShogiServer.Model.Requests
{
    public class MakeMoveRequest
    {
        public Guid SessionId { get; set; }
        public Guid PlayerId { get; set; }
        public string Token { get; set; } = null!;
        public string UsiMove { get; set; } = null!;
    }
}
