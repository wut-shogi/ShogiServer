using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace ShogiServer.WebApi.Hubs
{
    [SignalRHub]
    public class MatchmakingHub : Hub<IMatchmakingClient>
    {
        public async Task JoinLobby(JoinLobbyRequest request) {
            await Clients.All.SendLobby(2137);
        }

        public async Task Invite(InviteRequest request) {

        }

        public async Task AcceptInvitation(AcceptInvitationRequest request) {

        }

        public async Task RejectInvitation(RejectInvitationRequest request) {

        }
    }
}
