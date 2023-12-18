using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using ShogiServer.WebApi;
using ShogiServer.WebApi.Hubs;
using ShogiServer.WebApi.Model;

namespace ShogiServer.IntegrationTests.Hubs
{
    internal class ShogiHubTests
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
            List<Player>? result = null;
            connection1.On<List<Player>>(nameof(IShogiClient.SendLobby), response =>
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

        [Test]
        public async Task OnDisconnect_UpdatesLobby()
        {
            var nickname1 = "player1";
            List<Player>? result = null;
            connection1.On<List<Player>>(nameof(IShogiClient.SendLobby), response =>
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

        [Test]
        public async Task Invite_WhenPlayersReady_SendsInvitation()
        {
            var nickname1 = "player1";
            Invitation? result = null;
            connection1.On<Invitation>(nameof(IShogiClient.SendInvitation), response =>
            {
                result = response;
            });
            await connection1.StartAsync();
            await connection1.InvokeAsync(nameof(ShogiHub.JoinLobby), nickname1);

            var nickname2 = "player2";
            Player? player2 = null;
            connection2.On<Player>(nameof(IShogiClient.SendPlayer), response =>
            {
                player2 = response;
            });
            await connection2.StartAsync();
            await connection2.InvokeAsync(nameof(ShogiHub.JoinLobby), nickname2);
            await Task.Delay(2000);

            await connection2.InvokeAsync(nameof(ShogiHub.Invite), new InviteRequest(nickname1, player2!.Token));

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
            Player? player2 = null;
            connection2.On<Player>(nameof(IShogiClient.SendPlayer), response =>
            {
                player2 = response;
            });
            await connection2.StartAsync();
            await connection2.InvokeAsync(nameof(ShogiHub.JoinLobby), nickname2);
            await Task.Delay(2000);

            var act = async () => await connection2.InvokeAsync(nameof(ShogiHub.Invite), new InviteRequest(nickname1, player2!.Token));

            await act.Should().ThrowAsync<HubException>().Where(ex => ex.Message.Contains("nickname"));
        }

        [Test]
        public async Task Invite_WhenPlayerBeingInvited_ThrowsError()
        {
            var nickname1 = "player1";
            Player? player1 = null;
            connection1.On<Player>(nameof(IShogiClient.SendPlayer), response =>
            {
                player1 = response;
            });
            await connection1.StartAsync();
            await connection1.InvokeAsync(nameof(ShogiHub.JoinLobby), nickname1);
            await Task.Delay(2000);

            var nickname2 = "player2";
            await connection2.StartAsync();
            await connection2.InvokeAsync(nameof(ShogiHub.JoinLobby), nickname2);

            var nickname3 = "player3";
            Player? player3 = null;
            connection3.On<Player>(nameof(IShogiClient.SendPlayer), response =>
            {
                player3 = response;
            });
            await connection3.StartAsync();
            await connection3.InvokeAsync(nameof(ShogiHub.JoinLobby), nickname3);
            await Task.Delay(2000);

            await connection1.InvokeAsync(
                nameof(ShogiHub.Invite),
                new InviteRequest(nickname2, player1!.Token)
            );

            var act = async () => await connection3.InvokeAsync(
                nameof(ShogiHub.Invite),
                new InviteRequest(nickname2, player3!.Token)
            );

            await act.Should().ThrowAsync<HubException>().Where(ex => ex.Message.Contains("invite"));
        }

        [Test]
        public async Task RejectInvitation_WhenPlayerReject_SendsRejection()
        {
            // arrange
            var nickname1 = "player1";
            Player? player1 = null;
            connection1.On<Player>(nameof(IShogiClient.SendPlayer), response =>
            {
                player1 = response;
            });
            Invitation? invite = null;
            connection1.On<Invitation>(nameof(IShogiClient.SendInvitation), response =>
            {
                invite = response;
            });

            await connection1.StartAsync();
            await connection1.InvokeAsync(nameof(ShogiHub.JoinLobby), nickname1);

            var nickname2 = "player2";
            Player? player2 = null;
            connection2.On<Player>(nameof(IShogiClient.SendPlayer), response =>
            {
                player2 = response;
            });
            bool rejected = false;
            connection2.On(nameof(IShogiClient.SendRejection), () =>
            {
                rejected = true;
            });

            await connection2.StartAsync();
            await connection2.InvokeAsync(nameof(ShogiHub.JoinLobby), nickname2);
            await Task.Delay(2000);

            await connection2.InvokeAsync(nameof(ShogiHub.Invite), new InviteRequest(nickname1, player2!.Token));
            await Task.Delay(2000);

            // act
            await connection1.InvokeAsync(
                nameof(ShogiHub.RejectInvitation), 
                new RejectInvitationRequest(invite!.Id, player1!.Token)
            );
            await Task.Delay(2000);

            // assert
            rejected.Should().BeTrue();
        }

        [Test]
        public async Task RejectInvitation_WhenRejectingNonxistentInvitation_ThrowsError()
        {
            // arrange
            var nickname1 = "player1";
            Player? player1 = null;
            connection1.On<Player>(nameof(IShogiClient.SendPlayer), response =>
            {
                player1 = response;
            });
            Invitation? invite = null;
            connection1.On<Invitation>(nameof(IShogiClient.SendInvitation), response =>
            {
                invite = response;
            });

            await connection1.StartAsync();
            await connection1.InvokeAsync(nameof(ShogiHub.JoinLobby), nickname1);

            var nickname2 = "player2";
            Player? player2 = null;
            connection2.On<Player>(nameof(IShogiClient.SendPlayer), response =>
            {
                player2 = response;
            });

            await connection2.StartAsync();
            await connection2.InvokeAsync(nameof(ShogiHub.JoinLobby), nickname2);
            await Task.Delay(2000);

            await connection2.InvokeAsync(nameof(ShogiHub.Invite), new InviteRequest(nickname1, player2!.Token));
            await Task.Delay(2000);

            var nonExistentInviteId = Guid.NewGuid();

            // act
            var act = async () => await connection1.InvokeAsync(
                nameof(ShogiHub.RejectInvitation), 
                new RejectInvitationRequest(nonExistentInviteId, player1!.Token)
            );
            await Task.Delay(2000);

            // assert
            await act.Should().ThrowAsync<HubException>().Where(ex => ex.Message.Contains("exist"));
        }

        [Test]
        public async Task RejectInvitation_WhenRejectingNotOwnedInvitation_ThrowsError()
        {
            // arrange
            var nickname1 = "player1";
            Player? player1 = null;
            connection1.On<Player>(nameof(IShogiClient.SendPlayer), response =>
            {
                player1 = response;
            });
            Invitation? invite = null;
            connection1.On<Invitation>(nameof(IShogiClient.SendInvitation), response =>
            {
                invite = response;
            });

            await connection1.StartAsync();
            await connection1.InvokeAsync(nameof(ShogiHub.JoinLobby), nickname1);

            var nickname2 = "player2";
            Player? player2 = null;
            connection2.On<Player>(nameof(IShogiClient.SendPlayer), response =>
            {
                player2 = response;
            });

            await connection2.StartAsync();
            await connection2.InvokeAsync(nameof(ShogiHub.JoinLobby), nickname2);
            await Task.Delay(2000);

            await connection2.InvokeAsync(nameof(ShogiHub.Invite), new InviteRequest(nickname1, player2!.Token));
            await Task.Delay(2000);

            // act
            var act = async () => await connection2.InvokeAsync(
                nameof(ShogiHub.RejectInvitation), 
                new RejectInvitationRequest(invite!.Id, player2.Token)
            );
            await Task.Delay(2000);

            // assert
            await act.Should().ThrowAsync<HubException>().Where(ex => ex.Message.Contains("targeted"));
        }

        [Test]
        public async Task AcceptInvitation_WhenPlayersReady_CreatesGame()
        {
            var nickname1 = "player1";
            Player? player1 = null;
            connection1.On<Player>(nameof(IShogiClient.SendPlayer), response =>
            {
                player1 = response;
            });
            Invitation? invitation = null;
            connection1.On<Invitation>(nameof(IShogiClient.SendInvitation), response =>
            {
                invitation = response;
            });
            Game? game1 = null;
            connection1.On<Game>(nameof(IShogiClient.SendCreatedGame), response =>
            {
                game1 = response;
            });
            await connection1.StartAsync();
            await connection1.InvokeAsync(nameof(ShogiHub.JoinLobby), nickname1);

            var nickname2 = "player2";
            Player? player2 = null;
            connection2.On<Player>(nameof(IShogiClient.SendPlayer), response =>
            {
                player2 = response;
            });
            Game? game2 = null;
            connection2.On<Game>(nameof(IShogiClient.SendCreatedGame), response =>
            {
                game2 = response;
            });
            await connection2.StartAsync();
            await connection2.InvokeAsync(nameof(ShogiHub.JoinLobby), nickname2);
            await Task.Delay(2000);

            await connection2.InvokeAsync(nameof(ShogiHub.Invite), new InviteRequest(nickname1, player2!.Token));
            await Task.Delay(2000);

            await connection1.InvokeAsync(
                nameof(ShogiHub.AcceptInvitation),
                new AcceptInvitationRequest(invitation!.Id, player1!.Token)
            );
            await Task.Delay(2000);

            game1.Should().NotBeNull();
            game2.Should().NotBeNull();
            game1!.Id.Should().Be(game2!.Id);
        }

        [Test]
        public async Task MakeMove_WhenGameExists_UpdatesGameState()
        {
            var nickname1 = "player1";
            Player? player1 = null;
            connection1.On<Player>(nameof(IShogiClient.SendPlayer), response =>
            {
                player1 = response;
            });
            Invitation? invitation = null;
            connection1.On<Invitation>(nameof(IShogiClient.SendInvitation), response =>
            {
                invitation = response;
            });
            Game? game1 = null;
            connection1.On<Game>(nameof(IShogiClient.SendCreatedGame), response =>
            {
                game1 = response;
            });
            await connection1.StartAsync();
            await connection1.InvokeAsync(nameof(ShogiHub.JoinLobby), nickname1);

            var nickname2 = "player2";
            Player? player2 = null;
            connection2.On<Player>(nameof(IShogiClient.SendPlayer), response =>
            {
                player2 = response;
            });
            Game? game2 = null;
            connection2.On<Game>(nameof(IShogiClient.SendGameState), response =>
            {
                game2 = response;
            });
            await connection2.StartAsync();
            await connection2.InvokeAsync(nameof(ShogiHub.JoinLobby), nickname2);
            await Task.Delay(2000);

            await connection2.InvokeAsync(nameof(ShogiHub.Invite), new InviteRequest(nickname1, player2!.Token));
            await Task.Delay(2000);

            await connection1.InvokeAsync(
                nameof(ShogiHub.AcceptInvitation),
                new AcceptInvitationRequest(invitation!.Id, player1!.Token)
            );
            await Task.Delay(2000);

            var move = "test";
            await connection1.InvokeAsync(
                nameof(ShogiHub.MakeMove),
                new MakeMoveRequest(game1!.Id, player1!.Token, move)
            );
            await Task.Delay(2000);

            game1!.Id.Should().Be(game2!.Id);
            game2.BoardState.Should().Be(move);
        }

        [Test]
        public async Task CreateGameWithComputer_CreatesPlayerAndGame()
        {
            Player? player = null;
            connection1.On<Player>(nameof(IShogiClient.SendPlayer), response =>
            {
                player = response;
            });
            Game? game = null;
            connection1.On<Game>(nameof(IShogiClient.SendCreatedGame), response =>
            {
                game = response;
            });
            await connection1.StartAsync();


            await connection1.InvokeAsync(nameof(ShogiHub.CreateGameWithComputer));
            await Task.Delay(2000);

            player.Should().NotBeNull();
            player!.Nickname.Should().NotBeNullOrEmpty();
            game.Should().NotBeNull();
            game!.BlackId.Should().Be(player!.Id);
        }
    }
}
