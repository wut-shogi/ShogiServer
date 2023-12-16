namespace ShogiServer.WebApi.Model
{
    public class Game
    {
        public Guid Id { get; set; }
        
        public Guid WhiteId { get; set; }
        public Player White { get; set; } = null!;

        public Guid BlackId { get; set; }
        public Player Black { get; set; } = null!;
        public string BoardState { get; set; } = null!;
    }
}
