using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using ShogiServer.WebApi.Model;
using ShogiServer.WebApi.Services;
using ShogiServer.EngineWrapper;
using SignalRSwaggerGen.Attributes;

namespace ShogiServer.WebApi.Hubs
{
    public record InviteRequest
    {
        public string InvitedNickname { get; set; }
        public string Token { get; set; }
        public InviteRequest(string InvitedNickname, string Token)
        {
            this.InvitedNickname = InvitedNickname;
            this.Token = Token;
        }
    };

    public record RejectInvitationRequest
    {
        public Guid InvitationId { get; set; }
        public string Token { get; set; }
        public RejectInvitationRequest(Guid InvitationId, string Token)
        {
            this.InvitationId = InvitationId;
            this.Token = Token;
        }

    }

    public record CancelInvitationRequest
    {

        public Guid InvitationId { get; set; }
        public string Token { get; set; }
        public CancelInvitationRequest(Guid InvitationId, string Token)
        {
            this.InvitationId = InvitationId;
            this.Token = Token;
        }
    };

    public record AcceptInvitationRequest
    {
        public Guid InvitationId { get; set; }
        public string Token { get; set; }
        public AcceptInvitationRequest(Guid InvitationId, string Token)
        {
            this.InvitationId = InvitationId;
            this.Token = Token;
        }

    }

    public record MakeMoveRequest
    {
        public Guid GameId { get; set; }
        public string Token { get; set; }
        public string Move { get; set; }
        public MakeMoveRequest(Guid GameId, string Token, string Move)
        {
            this.GameId = GameId;
            this.Token = Token;
            this.Move = Move;
        }
    }

    [SignalRHub]
    public class ShogiHub : Hub<IShogiClient>
    {
        private readonly IRepositoryWrapper _repositories;

        public ShogiHub(IRepositoryWrapper repositories)
        {
            _repositories = repositories;
        }

        public async Task JoinLobby(string nickname)
        {
            var newPlayer = new Player
            {
                Id = Guid.NewGuid(),
                Nickname = nickname,
                ConnectionId = Context.ConnectionId,
                Token = Guid.NewGuid().ToString(),
                State = PlayerState.Ready,
            };

            try
            {
                _repositories.Players.Create(newPlayer);
                _repositories.Save();

                await Clients.Caller.SendPlayer(newPlayer);
                await Clients.All.SendLobby(AnonymousLobby());
            }
            catch (Exception ex)
            {
                throw new HubException(ex.Message);
            }
        }

        public async Task CreateGameWithComputer()
        {
            var newPlayer = new Player
            {
                Id = Guid.NewGuid(),
                Nickname = Guid.NewGuid().ToString(),
                ConnectionId = Context.ConnectionId,
                Token = Guid.NewGuid().ToString(),
                State = PlayerState.Playing,
            };

            try
            {
                var transaction = _repositories.BeginTransaction();

                _repositories.Players.Create(newPlayer);

                var newGame = new Game
                {
                    Id = Guid.NewGuid(),
                    BoardState = Engine.InitialPosition(),
                    BlackId = newPlayer.Id,
                    Type = GameType.PlayerVsComputer
                };

                _repositories.Games.Create(newGame);

                transaction!.Commit();
                _repositories.Save();

                newPlayer.GameAsBlack = null;
                await Clients.Caller.SendPlayer(newPlayer);
                await Clients.Caller.SendCreatedGame(GameDTO.FromDatabaseGame(newGame));
            }
            catch (Exception ex)
            {
                throw new HubException(ex.Message);
            }
        }

        private List<PlayerDTO> AnonymousLobby()
        {
            return _repositories
                .Players
                .FindAll()
                .Select(p => PlayerDTO.FromDatabasePlayer(p))
                .ToList();
        }

        private Player AuthenticatedPlayer(string token)
        {
            return _repositories
                .Players
                .FindByCondition(p => p.ConnectionId == Context.ConnectionId && p.Token == token)
                .FirstOrDefault() ??
                throw new HubException("Could not authenticate");
        }

        public async Task Invite(InviteRequest request)
        {
            var transaction = _repositories.BeginTransaction()!;
            try
            {
                var player = AuthenticatedPlayer(request.Token);

                var invitedPlayer = _repositories
                    .Players
                    .FindByCondition(p => p.Nickname == request.InvitedNickname)
                    .FirstOrDefault() ??
                    throw new HubException("There is no player with given nickname");

                if (player.State != PlayerState.Ready || invitedPlayer.State != PlayerState.Ready)
                {
                    throw new HubException("Players are not eligible for invite");
                }

                var invite = new Invitation
                {
                    Id = Guid.NewGuid(),
                    InvitedPlayerId = invitedPlayer.Id,
                    InvitingPlayerId = player.Id
                };
                _repositories.Invitations.Create(invite);

                player.SentInvitation = invite;
                player.State = PlayerState.Inviting;
                _repositories.Players.Update(player);

                invitedPlayer.ReceivedInvitation = invite;
                invitedPlayer.State = PlayerState.Inviting;
                _repositories.Players.Update(invitedPlayer);

                transaction.Commit();
                _repositories.Save();

                await Clients.Client(invitedPlayer.ConnectionId)
                    .SendInvitation(InvitationDTO.FromDatabaseInvitation(invite));
                await Clients.All.SendLobby(AnonymousLobby());
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task AcceptInvitation(AcceptInvitationRequest request)
        {
            var transaction = _repositories.BeginTransaction()!;
            try
            {
                var acceptingPlayer = AuthenticatedPlayer(request.Token);
                var invite = _repositories.Invitations.GetById(request.InvitationId) ??
                    throw new HubException("Invitation does not exist.");

                if (invite.InvitedPlayerId != acceptingPlayer.Id)
                {
                    throw new HubException("Invitation not targeted to this player.");
                }

                _repositories.Invitations.Delete(invite);

                var invitingPlayer = _repositories.Players.GetById(invite.InvitingPlayerId) ??
                    throw new HubException("Inviting player does not exist.");

                var newGame = new Game
                {
                    Id = Guid.NewGuid(),
                    BlackId = invitingPlayer.Id,
                    WhiteId = acceptingPlayer.Id,
                    BoardState = Engine.InitialPosition(),
                    Type = GameType.PlayerVsPlayer,
                };

                _repositories.Games.Create(newGame);

                invitingPlayer.SentInvitation = null;
                invitingPlayer.GameAsBlack = newGame;
                invitingPlayer.State = PlayerState.Playing;
                _repositories.Players.Update(invitingPlayer);

                acceptingPlayer.ReceivedInvitation = null;
                acceptingPlayer.GameAsWhite = newGame;
                acceptingPlayer.State = PlayerState.Playing;
                _repositories.Players.Update(acceptingPlayer);

                transaction.Commit();
                _repositories.Save();

                var gameDTO = GameDTO.FromDatabaseGame(newGame);

                await Clients.Client(invitingPlayer.ConnectionId).SendCreatedGame(gameDTO);
                await Clients.Client(acceptingPlayer.ConnectionId).SendCreatedGame(gameDTO);
                await Clients.All.SendLobby(AnonymousLobby());
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task RejectInvitation(RejectInvitationRequest request)
        {
            var transaction = _repositories.BeginTransaction()!;
            try
            {
                var player = AuthenticatedPlayer(request.Token);
                var invite = _repositories.Invitations.GetById(request.InvitationId) ??
                    throw new HubException("Invitation does not exist.");

                if (invite.InvitedPlayerId != player.Id)
                {
                    throw new HubException("Invitation not targeted to this player.");
                }

                _repositories.Invitations.Delete(invite);

                var invitingPlayer = _repositories.Players.GetById(invite.InvitingPlayerId) ??
                    throw new HubException("Inviting player does not exist.");

                invitingPlayer.SentInvitation = null;
                invitingPlayer.State = PlayerState.Ready;
                _repositories.Players.Update(invitingPlayer);

                player.ReceivedInvitation = null;
                player.State = PlayerState.Ready;
                _repositories.Players.Update(player);

                transaction.Commit();
                _repositories.Save();

                await Clients.Client(invitingPlayer.ConnectionId).SendRejection();
                await Clients.All.SendLobby(AnonymousLobby());
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // TODO - delete invitation
            try
            {
                var player = _repositories
                    .Players
                    .FindByCondition(p => p.ConnectionId == Context.ConnectionId)
                    .First();
                _repositories.Players.Delete(player);
                _repositories.Save();

                await Clients.All.SendLobby(AnonymousLobby());
            }
            catch (Exception ex)
            {
                throw new HubException(ex.Message);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task MakeMove(MakeMoveRequest request)
        {
            try
            {
                var game = _repositories.Games.GetById(request.GameId) ??
                    throw new HubException("Game does not exist.");

                await (game.Type switch
                {
                    GameType.PlayerVsPlayer => MakeMovePlayerVsPlayer(game, request),
                    GameType.PlayerVsComputer => MakeMovePlayerVsComputer(game, request.Move),
                    _ => throw new HubException("Invalid game type.")
                });
            }
            catch (Exception ex)
            {
                throw new HubException(ex.Message);
            }
        }

        private async Task MakeMovePlayerVsPlayer(Game game, MakeMoveRequest request)
        {

            var player = AuthenticatedPlayer(request.Token);
            var black = _repositories.Players.GetById(game.BlackId!.Value) ??
                throw new HubException("Black player does not exist.");

            var white = _repositories.Players.GetById(game.WhiteId!.Value) ??
                throw new HubException("White player does not exist.");

            if (!Engine.IsMoveValid(game.BoardState, request.Move))
            {
                throw new HubException("Invalid move.");
            }

            if (Engine.IsBlackTurn(game.BoardState) && player.Id != black.Id)
            {
                throw new HubException("It is black's turn.");
            }

            if (!Engine.IsBlackTurn(game.BoardState) && player.Id != white.Id)
            {
                throw new HubException("It is white's turn.");
            }

            game.BoardState = request.Move;
            _repositories.Games.Update(game);
            _repositories.Save();

            var gameDTO = GameDTO.FromDatabaseGame(game);

            await Clients.Client(black.ConnectionId).SendGameState(gameDTO);
            await Clients.Client(white.ConnectionId).SendGameState(gameDTO);
        }

        private async Task MakeMovePlayerVsComputer(Game game, string move)
        {
        }
    }
}
