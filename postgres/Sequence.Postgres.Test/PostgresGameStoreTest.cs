using Sequence.Core;
using Sequence.Core.CreateGame;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sequence.Postgres.Test
{
    [Collection(PostgresDockerContainerCollection.Name)]
    public sealed class PostgresGameStoreTest : PostgresTestBase
    {
        public PostgresGameStoreTest(PostgresDockerContainerFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task CanPersistNewGame()
        {
            // Given:
            var options = await CreateDatabaseAsync();
            var sut = new PostgresGameStore(options);
            var newGame = new NewGame(
                players: new PlayerList(
                    new NewPlayer(Player1, PlayerType.User),
                    new NewPlayer(Player2, PlayerType.User)),
                seed: new Seed(42));

            // When:
            var gameId = await sut.PersistNewGameAsync(newGame, CancellationToken.None);

            // Then:
            Assert.NotNull(gameId);
        }

        [Fact]
        public async Task CanPersistNewGameWithMultipleIdenticalBots()
        {
            // Given:
            var options = await CreateDatabaseAsync();
            var sut = new PostgresGameStore(options);
            var botType = new PlayerHandle("Dalvik");
            var newGame = new NewGame(
                players: new PlayerList(
                    new NewPlayer(botType, PlayerType.Bot),
                    new NewPlayer(botType, PlayerType.Bot)),
                seed: new Seed(42));

            // When:
            var gameId = await sut.PersistNewGameAsync(newGame, CancellationToken.None);

            // Then:
            Assert.NotNull(gameId);
        }
    }
}