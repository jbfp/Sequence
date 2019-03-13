using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Sequence.RealTime
{
    public sealed class GameHub : Hub<IGameHubClient>
    {
        private readonly ILogger _logger;

        public GameHub(ILogger<GameHub> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Identify(int playerId)
        {
            _logger.LogInformation(
                "Player with ID {PlayerId} has connected",
                playerId);

            var connectionId = Context.ConnectionId;
            var groupName = playerId.ToString();
            var cancellationToken = Context.ConnectionAborted;
            await Groups.AddToGroupAsync(connectionId, groupName, cancellationToken);
        }
    }
}
