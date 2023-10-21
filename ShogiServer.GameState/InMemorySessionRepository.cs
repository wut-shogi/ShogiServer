using ShogiServer.GameState.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShogiServer.GameState
{
    public class InMemorySessionRepository : ISessionRepository
    {
        public async Task<Session> Create(Player onBehalf)
        {
            throw new NotImplementedException();
        }

        public Session? Get(Guid sessionId)
        {
            throw new NotImplementedException();
        }
    }
}
