namespace ShogiServer.GameState.Model
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public PlayerColor Color { get; set; }
        public enum PlayerColor
        {
            White, Black
        }
    }
}
