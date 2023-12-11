using Microsoft.EntityFrameworkCore;

namespace ShogiServer.WebApi.Model
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<Game> Games { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>()
                .Property(p => p.State)
                .HasConversion<string>();

            modelBuilder.Entity<Player>()
                .HasOne(p => p.SentInvitation)
                .WithOne(i => i.InvitingPlayer)
                .HasForeignKey<Invitation>(i => i.InvitingPlayerId)
                .IsRequired(false);

            modelBuilder.Entity<Player>()
                .HasOne(p => p.ReceivedInvitation)
                .WithOne(i => i.InvitedPlayer)
                .HasForeignKey<Invitation>(i => i.InvitedPlayerId)
                .IsRequired(false);

            modelBuilder.Entity<Player>()
                .HasOne(p => p.GameAsBlack)
                .WithOne(g => g.Black)
                .HasForeignKey<Game>(g => g.BlackId)
                .IsRequired(false);

            modelBuilder.Entity<Player>()
                .HasOne(p => p.GameAsWhite)
                .WithOne(g => g.White)
                .HasForeignKey<Game>(g => g.WhiteId)
                .IsRequired(false);
        }
    }
}