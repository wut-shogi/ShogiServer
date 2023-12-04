using Microsoft.AspNetCore.SignalR;
using ShogiServer.WebApi.Model;
using ShogiServer.WebApi.Repositories;
using SignalRSwaggerGen.Attributes;

namespace ShogiServer.WebApi.Hubs
{
    [SignalRHub]
    public class MatchmakingHub : Hub<IMatchmakingClient>
    {
        private readonly ILobbyRepository _lobby;

        public MatchmakingHub(ILobbyRepository lobby)
        {
            _lobby = lobby;
        }

        public async Task JoinLobby(string nickname)
        {
            var newPlayer = new AuthenticatedPlayer
            {
                Nickname = nickname,
                ConnectionId = Context.ConnectionId
            };

            if (_lobby.Join(newPlayer))
            {
                await Clients.Caller.SendAuthenticatedPlayer(newPlayer);
                await Clients.All.SendLobby(_lobby.GetLobby());
            }
        }

        public async Task Invite(InviteRequest request)
        {
        }

        public async Task AcceptInvitation(AcceptInvitationRequest request)
        {
        }

        public async Task RejectInvitation(RejectInvitationRequest request)
        {
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _lobby.Leave(Context.ConnectionId);
            await Clients.All.SendLobby(_lobby.GetLobby());
            await base.OnDisconnectedAsync(exception);
        }
    }
}
