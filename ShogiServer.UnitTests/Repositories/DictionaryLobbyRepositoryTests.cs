using ShogiServer.WebApi.Model;
using ShogiServer.WebApi.Repositories;

namespace ShogiServer.UnitTests.Repositories
{
    internal class DictionaryLobbyRepositoryTests
    {
        private DictionaryLobbyRepository repository = null!;

        [SetUp]
        public void SetUp()
        {
            repository = new DictionaryLobbyRepository();
        }

        [Test]
        public void Join_WhenNicknameUnique_AddsNewPlayer()
        {
            repository.Join(new AuthenticatedPlayer
            {
                Nickname = "player1",
            });

            repository.Join(new AuthenticatedPlayer
            {
                Nickname = "player2",
            });

            repository.Join(new AuthenticatedPlayer { Nickname = "player3" }).Should().BeTrue();
        }

        [Test]
        public void Join_WhenNicknameTaken_ReturnsNull()
        {
            repository.Join(new AuthenticatedPlayer { Nickname = "player1" });
            repository.Join(new AuthenticatedPlayer { Nickname = "player2" });

            repository.Join(new AuthenticatedPlayer { Nickname = "player1" }).Should().BeFalse();
        }

        [Test]
        public void GetLobby_ReturnsListOfPlayers()
        {
            repository.Join(new AuthenticatedPlayer { Nickname = "player1" });
            repository.Join(new AuthenticatedPlayer { Nickname = "player2" });

            var lobby = repository.GetLobby();
            lobby.Should().NotBeNull();
            lobby.Count.Should().Be(2);
        }

        [Test]
        public void Leave_WhenPlayerPresent_LeavesLobby()
        {
            var player1 = new AuthenticatedPlayer { Nickname = "player1", ConnectionId = "1" };
            var player2 = new AuthenticatedPlayer { Nickname = "player2", ConnectionId = "2" };
            repository.Join(player1);
            repository.Join(player2);

            repository.Leave(player1.ConnectionId);

            repository.GetLobby().Count.Should().Be(1);
        }

        [Test]
        public void Leave_WhenPlayerNotPresent_DoNothing()
        {
            var player1 = new AuthenticatedPlayer { Nickname = "player1", ConnectionId = "1" };
            var player2 = new AuthenticatedPlayer { Nickname = "player2", ConnectionId = "2" };
            repository.Join(player1);
            repository.Join(player2);

            repository.Leave("3");

            repository.GetLobby().Count.Should().Be(2);
        }
    }
}
