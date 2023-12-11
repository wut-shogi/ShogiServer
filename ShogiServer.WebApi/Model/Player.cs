using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShogiServer.WebApi.Model
{
    [Index(nameof(Nickname), IsUnique = true)]
    [Index(nameof(ConnectionId), IsUnique = true)]
    public class Player
    {
        public Guid Id { get; set; }

        [MaxLength(100)]
        public string Nickname { get; set; } = null!;

        public string ConnectionId { get; set; } = null!;

        public string Token { get; set; } = null!;

        [Column(TypeName = "nvarchar(50)")]
        public PlayerState State { get; set; } = PlayerState.Ready;

        public Invitation? SentInvitation { get; set; } = null!;

        public Invitation? ReceivedInvitation { get; set; } = null!;

        public Game? GameAsBlack { get; set; } = null!;
        public Game? GameAsWhite { get; set; } = null!;
    }

    public enum PlayerState
    {
        Ready, Inviting, Playing
    }
}
