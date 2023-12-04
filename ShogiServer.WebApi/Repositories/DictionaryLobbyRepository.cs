using ShogiServer.WebApi.Model;
using System.Linq;

namespace ShogiServer.WebApi.Repositories
{
    public class DictionaryLobbyRepository : ILobbyRepository
    {
        private readonly object _lock = new();

        private class AuthenticatedPlayersStore
        {
            private readonly Dictionary<string, AuthenticatedPlayer> _nicknameToPlayers = new();
            private readonly Dictionary<string, string> _connectionsToNicknames = new();

            public bool ContainsConnection(string connectionId)
            {
                return _connectionsToNicknames.ContainsKey(connectionId);
            }

            public bool ContainsNickname(string nickname)
            {
                return _nicknameToPlayers.ContainsKey(nickname);
            }

            public bool Contains(string nickname, string connectionId)
            {
                return _connectionsToNicknames.ContainsKey(connectionId) || _nicknameToPlayers.ContainsKey(nickname);
            }

            public bool Add(AuthenticatedPlayer player)
            {
                if (!Contains(player.Nickname, player.ConnectionId))
                {
                    _nicknameToPlayers.Add(player.Nickname, player);
                    _connectionsToNicknames.Add(player.ConnectionId, player.Nickname);
                    return true;
                }
                return false;
            }

            public AuthenticatedPlayer? GetByNickname(string nickname)
            {
                return ContainsNickname(nickname) ? _nicknameToPlayers[nickname] : null;
            }

            public AuthenticatedPlayer? GetByConnection(string connectionId)
            {
                return ContainsConnection(connectionId) ? GetByNickname(_connectionsToNicknames[connectionId]) : null;
            }

            public List<AuthenticatedPlayer> GetValues()
            {
                return _nicknameToPlayers.Values.ToList();
            }

            public void RemoveByNickname(string nickname)
            {
                if (!ContainsNickname(nickname)) return;

                var connection = _nicknameToPlayers[nickname].ConnectionId;
                _connectionsToNicknames.Remove(connection);
                _nicknameToPlayers.Remove(nickname);
            }

            public void RemoveByConnection(string connectionId)
            {
                if (!ContainsConnection(connectionId)) return;

                var nickname = _connectionsToNicknames[connectionId];
                _connectionsToNicknames.Remove(connectionId);
                _nicknameToPlayers.Remove(nickname);
            }
        }

        private readonly AuthenticatedPlayersStore _lobby = new();

        public bool Join(AuthenticatedPlayer player)
        {
            lock (_lock)
            {
                return _lobby.Add(player);
            }
        }


        public List<Player> GetLobby()
        {
            lock (_lock)
            {
                return _lobby
                    .GetValues()
                    .Select(el => el.ToPlayer())
                    .ToList();
            }
        }

        public void Leave(string connectionId)
        {
            lock(_lock)
            {
                _lobby.RemoveByConnection(connectionId);
            }
        }
    }
}
