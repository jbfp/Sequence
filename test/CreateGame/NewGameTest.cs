using Sequence.CreateGame;
using System;
using Xunit;

namespace Sequence.Test.CreateGame
{
    public sealed class NewGameTest
    {
        private static readonly PlayerList _players = new PlayerList(
            randomFirstPlayer: false,
            new NewPlayer(
                new PlayerHandle("test 1"),
                PlayerType.User),
            new NewPlayer(
                    new PlayerHandle("test 2"),
                    PlayerType.User));
        private static readonly int _firstPlayerIndex = 0;
        private static readonly Seed _seed = new Seed(42);
        private static readonly BoardType _boardType = BoardType.OneEyedJack;
        private static readonly int _numSequencesToWin = 2;

        [Fact]
        public void NullArgs()
        {
            Assert.Throws<ArgumentNullException>(
                paramName: "players",
                testCode: () => new NewGame(players: null, _firstPlayerIndex, _seed, _boardType, _numSequencesToWin)
            );
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(42)]
        [InlineData(100)]
        public void BoardTypeMustBeDefined(int boardType)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                paramName: "boardType",
                testCode: () => new NewGame(_players, _firstPlayerIndex, _seed, (BoardType)boardType, _numSequencesToWin)
            );
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(42)]
        [InlineData(100)]
        public void NumSequencesToWinMustBeGreaterThanZeroAndLessThanFive(int numSequencesToWin)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                paramName: "numSequencesToWin",
                testCode: () => new NewGame(_players, _firstPlayerIndex, _seed, _boardType, numSequencesToWin)
            );
        }
    }
}
