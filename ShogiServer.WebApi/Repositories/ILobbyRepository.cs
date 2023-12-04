using ShogiServer.WebApi.Model;

namespace ShogiServer.WebApi.Repositories
{
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
    }
}
