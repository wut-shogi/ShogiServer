using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using ShogiServer.WebApi;
using ShogiServer.WebApi.Hubs;
using ShogiServer.WebApi.Model;

namespace ShogiServer.IntegrationTests.Hubs
{
    internal class MatchmakingHubTests
    {
        private WebApplicationFactory<Program> application = null!;
        private TestServer server = null!;
        private HubConnection connection1 = null!;
        private HubConnection connection2 = null!;
        private HubConnection connection3 = null!;

        [SetUp]
        public void Setup()
        {
            application = new WebApplicationFactory<Program>();
            server = application.Server;

            connection1 = new HubConnectionBuilder()
                .WithUrl(
                    "http://localhost/matchmaking",
                    o => o.HttpMessageHandlerFactory = _ => server.CreateHandler())
                .Build();

            connection2 = new HubConnectionBuilder()
                .WithUrl(
                    "http://localhost/matchmaking",
                    o => o.HttpMessageHandlerFactory = _ => server.CreateHandler())
                .Build();

            connection3 = new HubConnectionBuilder()
                .WithUrl(
                    "http://localhost/matchmaking",
                    o => o.HttpMessageHandlerFactory = _ => server.CreateHandler())
                .Build();
        }

        [Test]
        public async Task CanConnect()
        {
            await connection1.StartAsync();
            connection1.State.Should().Be(HubConnectionState.Connected);
        }

        [Test]
        public async Task JoinLobby_CreatesPlayer()
        {
            var nickname = "player1";
            AuthenticatedPlayer? result = null;
            connection1.On<AuthenticatedPlayer>(nameof(IMatchmakingClient.SendAuthenticatedPlayer), response =>
            {
                result = response;
            });
            await connection1.StartAsync();

            await connection1.InvokeAsync(nameof(MatchmakingHub.JoinLobby), nickname);
            
            await Task.Delay(2000);
            result.Should().NotBeNull();
            result?.Nickname.Should().Be(nickname);
            result?.Token.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task JoinLobby_UpdatesLobby()
        {
            var nickname1 = "player1";
            List<Player>? result = null;
            connection1.On<List<Player>>(nameof(IMatchmakingClient.SendLobby), response =>
            {
                result = response;
            });

            await connection1.StartAsync();
            await connection1.InvokeAsync(nameof(MatchmakingHub.JoinLobby), nickname1);

            var nickname2 = "player2";
            await connection2.StartAsync();
            await connection2.InvokeAsync(nameof(MatchmakingHub.JoinLobby), nickname2);
                        
            await Task.Delay(2000);
            result.Should().NotBeNull();
            result?.Count.Should().Be(2);
        }

        [Test]
        public async Task OnDisconnect_UpdatesLobby()
        {
            var nickname1 = "player1";
            List<Player>? result = null;
            connection1.On<List<Player>>(nameof(IMatchmakingClient.SendLobby), response =>
            {
                result = response;
            });

            await connection1.StartAsync();
            await connection1.InvokeAsync(nameof(MatchmakingHub.JoinLobby), nickname1);

            var nickname2 = "player2";
            await connection2.StartAsync();
            await connection2.InvokeAsync(nameof(MatchmakingHub.JoinLobby), nickname2);

            await connection2.StopAsync();
            await connection2.DisposeAsync();

            connection2.State.Should().Be(HubConnectionState.Disconnected);

            await Task.Delay(2000);
            result.Should().NotBeNull();
            result?.Count.Should().Be(1);
        }

        [Test]
        public async Task Invite_WhenPlayersReady_SendsInvitation()
        {
            var nickname1 = "player1";
            Invitation? result = null;
            connection1.On<Invitation>(nameof(IMatchmakingClient.SendInvitation), response =>
            {
                result = response;
            });

            await connection1.StartAsync();
            await connection1.InvokeAsync(nameof(MatchmakingHub.JoinLobby), nickname1);

            var nickname2 = "player2";
            AuthenticatedPlayer? player2 = null;
            connection2.On<AuthenticatedPlayer>(nameof(IMatchmakingClient.SendAuthenticatedPlayer), response =>
            {
                player2 = response;
            });
            await connection2.StartAsync();
            await connection2.InvokeAsync(nameof(MatchmakingHub.JoinLobby), nickname2);
            await Task.Delay(2000);

            await connection2.InvokeAsync(nameof(MatchmakingHub.Invite), new InviteRequest(nickname1, player2!.Token));

            await Task.Delay(2000);
            result.Should().NotBeNull();
            result?.InvitedPlayer.Nickname.Should().Be(nickname1);
            result?.InvitingPlayer.Nickname.Should().Be(nickname2);
        }

        [Test]
        public async Task Invite_WhenPlayerNonexistent_ThrowsError()
        {
            var nickname1 = "player1";

            var nickname2 = "player2";
            AuthenticatedPlayer? player2 = null;
            connection2.On<AuthenticatedPlayer>(nameof(IMatchmakingClient.SendAuthenticatedPlayer), response =>
            {
                player2 = response;
            });
            await connection2.StartAsync();
            await connection2.InvokeAsync(nameof(MatchmakingHub.JoinLobby), nickname2);
            await Task.Delay(2000);

            var act = async () => await connection2.InvokeAsync(nameof(MatchmakingHub.Invite), new InviteRequest(nickname1, player2!.Token));
            
            await act.Should().ThrowAsync<HubException>().Where(ex => ex.Message.Contains("invite"));
        }

        [Test]
        public async Task Invite_WhenPlayerBeingInvited_ThrowsError()
        {
            var nickname1 = "player1";
            AuthenticatedPlayer? player1 = null;
            connection1.On<AuthenticatedPlayer>(nameof(IMatchmakingClient.SendAuthenticatedPlayer), response => 
            {
                player1 = response;
            });
            await connection1.StartAsync();
            await connection1.InvokeAsync(nameof(MatchmakingHub.JoinLobby), nickname1);
            await Task.Delay(2000);

            var nickname2 = "player2";
            await connection2.StartAsync();
            await connection2.InvokeAsync(nameof(MatchmakingHub.JoinLobby), nickname2);

            var nickname3 = "player3";
            AuthenticatedPlayer? player3 = null;
            connection3.On<AuthenticatedPlayer>(nameof(IMatchmakingClient.SendAuthenticatedPlayer), response => 
            {
                player3 = response;
            });
            await connection3.StartAsync();
            await connection3.InvokeAsync(nameof(MatchmakingHub.JoinLobby), nickname3);
            await Task.Delay(2000);
            
            await connection1.InvokeAsync(
                nameof(MatchmakingHub.Invite),
                new InviteRequest(nickname2, player1!.Token)
            );

            var act = async () => await connection3.InvokeAsync(
                nameof(MatchmakingHub.Invite),
                new InviteRequest(nickname2, player3!.Token)
            );

            await act.Should().ThrowAsync<HubException>().Where(ex => ex.Message.Contains("invite"));
        }
    }
}
