using Microsoft.EntityFrameworkCore;
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

        public override Game? GetById(Guid id)
        {
            return DatabaseContext
                .Games
                .Include(g => g.Black)
                .Include(g => g.White)
                .Where(g => g.Id == id)
                .FirstOrDefault();
        }
    }
}
