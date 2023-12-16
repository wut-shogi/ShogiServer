using ShogiServer.WebApi.Model;

namespace ShogiServer.WebApi.Services
{
    public interface IPlayerRepository : IRepositoryBase<Player>
    {
    }

    public class PlayerRepository : RepositoryBase<Player>, IPlayerRepository
    {
        public PlayerRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
