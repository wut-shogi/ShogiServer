using Microsoft.AspNetCore.Mvc.Testing;
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
            result.Should().NotBeNull();
            result?.Count.Should().Be(1);
        }
    }
}
