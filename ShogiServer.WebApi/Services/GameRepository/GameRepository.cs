using ShogiServer.WebApi.Model;

namespace ShogiServer.WebApi.Services
{
    public interface IGameRepository : IRepositoryBase<Game>
    {
    }

    public class GameRepository : RepositoryBase<Game>, IGameRepository
    {
        public GameRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
