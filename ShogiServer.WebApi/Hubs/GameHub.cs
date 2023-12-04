using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace ShogiServer.WebApi.Hubs
{
    [SignalRHub]
    public class GameHub : Hub<IGameClient>
    {
        public async Task MakeMove(MakeMoveRequest request)
        {
            
        }
    }

    public class MakeMoveRequest
    {
        public Guid GameId { get; set; }
        public string Move { get; set; }
    }

    public class Move
    {
        public string UsiMove { get; set; }
    }
}
