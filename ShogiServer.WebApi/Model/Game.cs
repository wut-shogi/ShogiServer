using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShogiServer.WebApi.Model
{
    public class Game
    {
        public Guid Id { get; set; }
        
        public Guid? WhiteId { get; set; }
        public Player? White { get; set; }

        public Guid? BlackId { get; set; }
        public Player? Black { get; set; }

        public string BoardState { get; set; } = null!;

        [Column(TypeName = "nvarchar(50)")]
        public GameType Type { get; set; }
    }

    public enum GameType
    {
        PlayerVsPlayer,
        PlayerVsComputer
    }
}
