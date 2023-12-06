using ShogiServer.WebApi.Model;
using System.Linq;

namespace ShogiServer.WebApi.Services
{
    /// <summary>
    /// ILobbyRepository implementation. Uses dictionary as inner data container.
    /// As it is an in-memory container, data is not saved between applications runs.
    /// </summary>
    public class DictionaryLobbyRepository : ILobbyRepository
    {
        private readonly object _lock = new();

        /// <summary>
        /// Wrapper for dictionary of AuthenticatedPlayer with two keys,
        /// which are ConnectionId and Nickname.
        /// </summary>
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

        private readonly Dictionary<Guid, Invitation> _invitations = new();

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
            lock (_lock)
            {
                _lobby.RemoveByConnection(connectionId);
            }
        }

        public Player? GetPlayer(string nickname)
        {
            lock (_lock)
            {
                return _lobby.GetByNickname(nickname)?.ToPlayer();
            }
        }

        public AuthenticatedPlayer? GetAuthenticatedPlayer(string connectionId, string token)
        {
            lock (_lock)
            {
                var player = _lobby.GetByConnection(connectionId);
                return player?.Token == token ? player : null;
            }
        }

        public Invitation? CreateInvitation(string invitingConnection, string invitedNickname)
        {
            lock (_lock)
            {
                var inviting = _lobby.GetByConnection(invitingConnection);
                var invited = _lobby.GetByNickname(invitedNickname);

                if (invited?.State != PlayerState.Ready || inviting?.State != PlayerState.Ready)
                {
                    return null;
                }

                inviting.State = PlayerState.Inviting;
                invited.State = PlayerState.Invited;

                var invitation = new Invitation
                {
                    InvitingPlayer = inviting.ToPlayer(),
                    InvitedPlayer = invited.ToPlayer(),
                };

                _invitations.Add(invitation.Id, invitation);
                return invitation;
            }
        }

        public Invitation? GetInvitation(Guid invitationId)
        {
            lock (_lock)
            {
                return _invitations.ContainsKey(invitationId) ? _invitations[invitationId] : null;
            }
        }

        public void RemoveInvitation(Guid invitationId)
        {
            lock (_lock)
            {
                if (!_invitations.ContainsKey(invitationId)) return;

                var invitation = _invitations[invitationId];
                _invitations.Remove(invitationId);

                var player = _lobby.GetByConnection(invitation.InvitingPlayer.ConnectionId);
                if (player != null)
                {
                    player.State = PlayerState.Ready;
                }
                
                player = _lobby.GetByConnection(invitation.InvitedPlayer.ConnectionId);
                if (player != null)
                {
                    player.State = PlayerState.Ready;
                }
            }
        }

        public void AcceptInvitation(Guid invitationId)
        {
            throw new NotImplementedException();
        }
    }
}
