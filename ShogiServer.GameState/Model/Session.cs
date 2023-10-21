namespace ShogiServer.GameState.Model
{
    public class Session
    {
        public Guid Id { get; set; }
        public GameState GameState { get; set; } = null!;
        public Player WhitePlayer { get; set; } = null!;
        public Player BlackPlayer { get; set; } = null!;
    }
}
