using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using ShogiServer.WebApi.Hubs;
using ShogiServer.WebApi.Model;

namespace ShogiServer.IntegrationTests.Hubs.ShogiHubTests
{
    internal class GameTests : ShogiHubTests
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
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
            InvitationDTO? invitation = null;
            connection1.On<InvitationDTO>(nameof(IShogiClient.SendInvitation), response =>
            {
                invitation = response;
            });
            GameDTO? game1 = null;
            connection1.On<GameDTO>(nameof(IShogiClient.SendGameState), response =>
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
            GameDTO? game2 = null;
            connection2.On<GameDTO>(nameof(IShogiClient.SendCreatedGame), response =>
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

            var move = "1g1f";
            //var boardAfterMove = "lnsgkgsnl/1r5b1/ppppppppp/9/9/P8/1PPPPPPPP/1B5R1/LNSGKGSNL w - 2"; <= TODO
            await connection2.InvokeAsync(
                nameof(ShogiHub.MakeMove),
                new MakeMoveRequest(game2!.Id, player2!.Token, move)
            );
            await Task.Delay(2000);

            game1!.Id.Should().Be(game2!.Id);
            game1!.BoardState.Should().Be(move);
            //game2.BoardState.Should().Be(boardAfterMove); <= TODO
        }

        [Test]
        public async Task MakeMove_WhenWrongPlayerMoves_ThrowsException()
        {
            var nickname1 = "player1";
            Player? player1 = null;
            connection1.On<Player>(nameof(IShogiClient.SendPlayer), response =>
            {
                player1 = response;
            });
            InvitationDTO? invitation = null;
            connection1.On<InvitationDTO>(nameof(IShogiClient.SendInvitation), response =>
            {
                invitation = response;
            });
            GameDTO? game1 = null;
            connection1.On<GameDTO>(nameof(IShogiClient.SendCreatedGame), response =>
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
            GameDTO? game2 = null;
            connection2.On<GameDTO>(nameof(IShogiClient.SendGameState), response =>
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

            var move = "1g1f";
            //var boardAfterMove = "lnsgkgsnl/1r5b1/ppppppppp/9/9/P8/1PPPPPPPP/1B5R1/LNSGKGSNL w - 2"; <= TODO
            var act = async () => await connection1.InvokeAsync(
                nameof(ShogiHub.MakeMove),
                new MakeMoveRequest(game1!.Id, player1!.Token, move)
            );

            await act.Should().ThrowAsync<HubException>().Where(ex => ex.Message.Contains("It is"));
        }

        [Test]
        public async Task MakeMove_WhenInvalidMovePlayed_ThrowsException()
        {
            var nickname1 = "player1";
            Player? player1 = null;
            connection1.On<Player>(nameof(IShogiClient.SendPlayer), response =>
            {
                player1 = response;
            });
            InvitationDTO? invitation = null;
            connection1.On<InvitationDTO>(nameof(IShogiClient.SendInvitation), response =>
            {
                invitation = response;
            });
            GameDTO? game1 = null;
            connection1.On<GameDTO>(nameof(IShogiClient.SendGameState), response =>
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
            GameDTO? game2 = null;
            connection2.On<GameDTO>(nameof(IShogiClient.SendCreatedGame), response =>
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

            var move = "aaaa";
            //var boardAfterMove = "lnsgkgsnl/1r5b1/ppppppppp/9/9/P8/1PPPPPPPP/1B5R1/LNSGKGSNL w - 2"; <= TODO
            var act = async () => await connection2.InvokeAsync(
                nameof(ShogiHub.MakeMove),
                new MakeMoveRequest(game2!.Id, player2!.Token, move)
            );

            await act.Should().ThrowAsync<HubException>().Where(ex => ex.Message.Contains("Invalid move"));
        }

        [Test]
        public async Task CreateGameWithComputer_CreatesPlayerAndGame()
        {
            Player? player = null;
            connection1.On<Player>(nameof(IShogiClient.SendPlayer), response =>
            {
                player = response;
            });
            GameDTO? game = null;
            connection1.On<GameDTO>(nameof(IShogiClient.SendCreatedGame), response =>
            {
                game = response;
            });
            await connection1.StartAsync();

            await connection1.InvokeAsync(nameof(ShogiHub.CreateGameWithComputer));
            
            await Task.Delay(2000);

            player.Should().NotBeNull();
            player!.Nickname.Should().NotBeNullOrEmpty();
            game.Should().NotBeNull();
            game!.BlackPlayer?.Id.Should().Be(player!.Id);
        }
    }
}
