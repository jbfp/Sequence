using System.Threading;
using System.Threading.Tasks;

namespace Sequence.PlayCard
{
    public interface IGameEventStore
    {
        Task AddEventAsync(GameId gameId, GameEvent gameEvent, CancellationToken cancellationToken);
    }
}
