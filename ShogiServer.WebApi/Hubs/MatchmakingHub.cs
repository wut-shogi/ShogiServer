using Microsoft.AspNetCore.SignalR;
using ShogiServer.WebApi.Model;
using ShogiServer.WebApi.Services;
using SignalRSwaggerGen.Attributes;

namespace ShogiServer.WebApi.Hubs
{
    public record InviteRequest(string InvitedNickname, string Token);
    public record RejectInvitationRequest(Guid InvitationId, string Token);

    [SignalRHub]
    public class MatchmakingHub : Hub<IMatchmakingClient>
    {
        private readonly ILobbyRepository _lobby;

        public MatchmakingHub(ILobbyRepository lobby)
        {
            _lobby = lobby;
        }
        
        private void AuthenticateOrThrow(string token)
        {
            _ = _lobby.GetAuthenticatedPlayer(Context.ConnectionId, token) ??
                throw new HubException("Could not authenticate request.");
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
            else
            {
                throw new HubException("Could not join lobby.");
            }
        }

        public async Task Invite(InviteRequest request)
        {
            AuthenticateOrThrow(request.Token);

            var invite = _lobby.Invite(Context.ConnectionId, request.InvitedNickname) ??
                throw new HubException("Could not send invite.");

            await Clients.Client(invite.InvitedPlayer.ConnectionId).SendInvitation(invite);
            await Clients.All.SendLobby(_lobby.GetLobby());
        }

        public async Task AcceptInvitation(AcceptInvitationRequest request)
        {
        }

        public async Task RejectInvitation(RejectInvitationRequest request)
        {
            AuthenticateOrThrow(request.Token);

            

            await Clients.All.SendLobby(_lobby.GetLobby());
        }

        public async Task CancelInvitation(AcceptInvitationRequest request)
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
