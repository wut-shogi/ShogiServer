using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using ShogiServer.WebApi.Hubs;
using ShogiServer.WebApi.Model;

namespace ShogiServer.IntegrationTests.Hubs.ShogiHubTests
{
    internal class JoinLobbyTests : ShogiHubTests
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
        }

        [Test]
        public async Task JoinLobby_CreatesPlayer()
        {
            var nickname = "player1";
            Player? result = null;
            connection1.On<Player>(nameof(IShogiClient.SendPlayer), response =>
            {
                result = response;
            });
            await connection1.StartAsync();

            await connection1.InvokeAsync(nameof(ShogiHub.JoinLobby), nickname);

            await Task.Delay(2000);
            result.Should().NotBeNull();
            result?.Nickname.Should().Be(nickname);
            result?.Token.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task JoinLobby_UpdatesLobby()
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

            await Task.Delay(2000);
            result.Should().NotBeNull();
            result?.Count.Should().Be(2);
        }
    }
}
