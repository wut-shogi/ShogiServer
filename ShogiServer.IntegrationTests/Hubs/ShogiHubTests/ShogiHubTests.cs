using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using ShogiServer.WebApi;
using ShogiServer.WebApi.Hubs;

namespace ShogiServer.IntegrationTests.Hubs.ShogiHubTests
{
    internal class ShogiHubTests
    {
        protected WebApplicationFactory<Program> application = null!;
        protected TestServer server = null!;
        protected HubConnection connection1 = null!;
        protected HubConnection connection2 = null!;
        protected HubConnection connection3 = null!;

        [SetUp]
        public virtual void Setup()
        {
            application = new WebApplicationFactory<Program>();
            server = application.Server;

            connection1 = new HubConnectionBuilder()
                .WithUrl(
                    "http://localhost/shogi-hub",
                    o => o.HttpMessageHandlerFactory = _ => server.CreateHandler())
                .WithAutomaticReconnect()
                .Build();

            connection2 = new HubConnectionBuilder()
                .WithUrl(
                    "http://localhost/shogi-hub",
                    o => o.HttpMessageHandlerFactory = _ => server.CreateHandler())
                .WithAutomaticReconnect()
                .Build();

            connection3 = new HubConnectionBuilder()
                .WithUrl(
                    "http://localhost/shogi-hub",
                    o => o.HttpMessageHandlerFactory = _ => server.CreateHandler())
                .WithAutomaticReconnect()
                .Build();
        }

        [Test]
        public async Task CanConnect()
        {
            await connection1.StartAsync();
            connection1.State.Should().Be(HubConnectionState.Connected);
        }

        [Test]
        public async Task OnDisconnect_UpdatesLobby()
        {
            var nickname1 = "player1";
            List<PlayerDTO>? result = null;
            connection1.On<List<PlayerDTO>>(nameof(IShogiClient.SendLobby), response =>
            {
                result = response;
            });

            await connection1.StartAsync();
            await connection1.InvokeAsync(nameof(ShogiHub.JoinLobby), nickname1);

            var nickname2 = "player2";
            await connection2.StartAsync();
            await connection2.InvokeAsync(nameof(ShogiHub.JoinLobby), nickname2);

            await connection2.StopAsync();
            await connection2.DisposeAsync();

            connection2.State.Should().Be(HubConnectionState.Disconnected);

            await Task.Delay(2000);
            result.Should().NotBeNull();
            result?.Count.Should().Be(1);
        }
    }
}
