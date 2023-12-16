using Microsoft.EntityFrameworkCore.Storage;
using ShogiServer.WebApi.Model;

namespace ShogiServer.WebApi.Services
{
    public interface IRepositoryWrapper
    {
        IGameRepository Games { get; }
        IPlayerRepository Players { get; }
        IInvitationRepository Invitations { get; }
        IDbContextTransaction? BeginTransaction();
        void Save();
    }

    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly DatabaseContext _context;

        private readonly IGameRepository _games;
        public IGameRepository Games => _games;


        private readonly IPlayerRepository _players;
        public IPlayerRepository Players => _players;

        private IInvitationRepository _invitations;
        public IInvitationRepository Invitations => _invitations;

        public RepositoryWrapper(DatabaseContext context)
        {
            _context = context;
            _players = new PlayerRepository(context);
            _games = new GameRepository(context);
            _invitations = new InvitationRepository(context);
        }

        public IDbContextTransaction? BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
