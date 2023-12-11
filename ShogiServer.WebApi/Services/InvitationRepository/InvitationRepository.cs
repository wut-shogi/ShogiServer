using ShogiServer.WebApi.Model;

namespace ShogiServer.WebApi.Services
{
    public interface IInvitationRepository : IRepositoryBase<Invitation>
    {
    }

    public class InvitationRepository : RepositoryBase<Invitation>, IInvitationRepository
    {
        public InvitationRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
