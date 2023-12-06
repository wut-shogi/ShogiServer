using ShogiServer.WebApi.Model;

namespace ShogiServer.WebApi.Services
{
    /// <summary>
    /// Interface of lobby data container. Supports all operations necessary
    /// for lobby, such as joining, leaving, searching, inviting etc.
    /// Its operations should be atomic.
    /// </summary>
    public interface ILobbyRepository
    {
        /// <summary>
        /// Create new player with given nickname
        /// </summary>
        /// <param name="nickname">New player. Nickname and ConnectionId have to be unique in current lobby </param>
        /// <returns>True if player was added, false otherwise</returns>
        bool Join(AuthenticatedPlayer player);

        /// <summary>
        /// Retrieve copy of current lobby state
        /// </summary>
        /// <returns>List of players in lobby</returns>
        List<Player> GetLobby();

        /// <summary>
        /// Remove player from lobby or do nothing, if player not present.
        /// </summary>
        /// <param name="nickname">Connection of leaving player</param>
        void Leave(string connectionId);

        /// <summary>
        /// Retrieve player by nickname
        /// </summary>
        /// <returns>Found player or null, if not found</returns>
        Player? GetPlayer(string nickname);

        /// <summary>
        /// Retrieve authencitated player by connection
        /// </summary>
        /// <returns>Found player or null, if not found</returns>
        AuthenticatedPlayer? GetAuthenticatedPlayer(string connectionId, string token);

        /// <summary>
        /// Create invitation and lock players' state.
        /// </summary>
        /// <param name="invitingConnection"></param>
        /// <param name="invitedNickname"></param>
        /// <returns>Created invitation or null, if operation failed</returns>
        Invitation? CreateInvitation(string invitingConnection, string invitedNickname);

        /// <summary>
        /// Get invitation by id.
        /// </summary>
        /// <param name="invitationId"></param>
        /// <returns>Invitation or null, if operation failed</returns>
        Invitation? GetInvitation(Guid invitationId);

        /// <summary>
        /// Remove invitation and unlock players' state.
        /// </summary>
        /// <param name="invitationId"></param>
        void RemoveInvitation(Guid invitationId);

        /// <summary>
        /// Accept invitation, create game and removeplayers from lobby.
        /// </summary>
        /// <param name="invitationId"></param>
        /// <returns>Created game or null, if operation failed</returns>
        void AcceptInvitation(Guid invitationId);
    }
}
