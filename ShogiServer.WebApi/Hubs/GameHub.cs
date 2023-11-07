using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace ShogiServer.WebApi.Hubs
{
    [SignalRHub]
    public class GameHub : Hub<IGameClient>
    {
        public async Task MakeMove()
        {
            
        }
    }
}
