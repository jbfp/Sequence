using System;
using System.Collections.Immutable;
using System.Linq;

namespace Sequence
{
    public sealed class GameState
    {
        private static readonly ImmutableDictionary<int, ImmutableArray<Team>> _teams =
            ImmutableDictionary<int, ImmutableArray<Team>>
                .Empty
                .Add(2, ImmutableArray.Create(Team.Red, Team.Green))
                .Add(3, ImmutableArray.Create(Team.Red, Team.Green, Team.Blue))
                .Add(4, ImmutableArray.Create(Team.Red, Team.Green,
                                              Team.Red, Team.Green))
                .Add(6, ImmutableArray.Create(Team.Red, Team.Green, Team.Blue,
                                              Team.Red, Team.Green, Team.Blue));

        private readonly GameInit _init;

        public GameState(GameInit init, params GameEvent[] gameEvents)
        {
            if (init == null)
            {
                throw new ArgumentNullException(nameof(init));
            }

            if (gameEvents == null)
            {
                throw new ArgumentNullException(nameof(gameEvents));
            }

            _init = init;

            var numPlayers = init.Players.Count;
            var deck = new Deck(init.Seed, numPlayers);
            var hands = deck.DealHands();
            var teams = _teams[numPlayers];

            for (int i = 0; i < numPlayers; i++)
            {
                Player player = init.Players[i];
                PlayerHandByIdx = PlayerHandByIdx.Add(hands[i]);
                PlayerHandleByIdx = PlayerHandleByIdx.Add(player.Handle);
                PlayerIdByIdx = PlayerIdByIdx.Add(player.Id);
                PlayerTeamByIdx = PlayerTeamByIdx.Add(teams[i]); // TODO: Get team from init.
                PlayerTypeByIdx = PlayerTypeByIdx.Add(player.Type);
            }

            BoardType = init.BoardType.Create();
            CurrentPlayerId = init.FirstPlayerId;
            Deck = deck.ToImmutableList();
            NumberOfPlayers = numPlayers;
            WinCondition = init.NumberOfSequencesToWin;

            foreach (var gameEvent in gameEvents)
            {
                var cardDrawn = gameEvent.CardDrawn;
                var cardUsed = gameEvent.CardUsed;
                var playerIdx = PlayerIdByIdx.IndexOf(gameEvent.ByPlayerId);
                var playerHand = PlayerHandByIdx[playerIdx].Remove(cardUsed);

                if (gameEvent.CardDrawn != null)
                {
                    Deck = Deck.Remove(gameEvent.CardDrawn);
                    playerHand = playerHand.Add(gameEvent.CardDrawn);
                }

                PlayerHandByIdx = PlayerHandByIdx.SetItem(playerIdx, playerHand);

                if (gameEvent.Chip.HasValue)
                {
                    Chips = Chips.Add(gameEvent.Coord, gameEvent.Chip.Value);
                }
                else
                {
                    Chips = Chips.Remove(gameEvent.Coord);
                }

                CurrentPlayerId = gameEvent.NextPlayerId;
                Discards = Discards.Push(cardUsed);
                GameEvents = GameEvents.Add(gameEvent);
                Version = gameEvent.Index;

                if (gameEvent.Sequence != null)
                {
                    Sequences = Sequences.Add(gameEvent.Sequence);

                    foreach (var coord in gameEvent.Sequence.Coords)
                    {
                        CoordsInSequence = CoordsInSequence.Add(coord);
                    }
                }

                Winner = gameEvent.Winner;
            }
        }

        public IBoardType BoardType { get; }
        public IImmutableDictionary<Coord, Team> Chips { get; } =
            ImmutableDictionary<Coord, Team>.Empty;
        public IImmutableSet<Coord> CoordsInSequence { get; } = ImmutableHashSet<Coord>.Empty;
        public PlayerId CurrentPlayerId { get; }
        public IImmutableList<Card> Deck { get; } = ImmutableList<Card>.Empty;
        public IImmutableStack<Card> Discards { get; } = ImmutableStack<Card>.Empty;
        public IImmutableList<GameEvent> GameEvents { get; } = ImmutableList<GameEvent>.Empty;
        public int NumberOfPlayers { get; }
        public IImmutableList<IImmutableList<Card>> PlayerHandByIdx { get; } =
            ImmutableList<IImmutableList<Card>>.Empty;
        public IImmutableList<PlayerHandle> PlayerHandleByIdx { get; } =
            ImmutableList<PlayerHandle>.Empty;
        public IImmutableList<PlayerId> PlayerIdByIdx { get; } =
            ImmutableList<PlayerId>.Empty;
        public IImmutableList<Team> PlayerTeamByIdx { get; } = ImmutableList<Team>.Empty;
        public IImmutableList<PlayerType> PlayerTypeByIdx { get; } =
            ImmutableList<PlayerType>.Empty;
        public IImmutableList<Seq> Sequences { get; } = ImmutableList<Seq>.Empty;
        public int Version { get; }
        public int WinCondition { get; }
        public Team? Winner { get; }

        public static GameState Apply(GameState state, GameEvent gameEvent)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (gameEvent == null)
            {
                throw new ArgumentNullException(nameof(gameEvent));
            }

            return new GameState(state._init, state.GameEvents.Add(gameEvent).ToArray());
        }
    }
}
