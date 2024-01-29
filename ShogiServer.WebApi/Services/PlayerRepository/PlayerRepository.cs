using Microsoft.EntityFrameworkCore;
using ShogiServer.WebApi.Model;

namespace ShogiServer.WebApi.Services
{
    public interface IPlayerRepository : IRepositoryBase<Player>
    {
        Player? GetByConnectionId(string connectionId);
    }

    public class PlayerRepository : RepositoryBase<Player>, IPlayerRepository
    {
        public PlayerRepository(DatabaseContext context) : base(context)
        {
        }

        public Player? GetByConnectionId(string connectionId)
        {
            return DatabaseContext
                .Players
                .Include(p => p.ReceivedInvitation)
                .Include(p => p.SentInvitation)
                .Include(p => p.SentInvitation)
                .Include(p => p.SentInvitation)
                .Where(p => p.ConnectionId == connectionId)
                .FirstOrDefault();
        }
    }
}

