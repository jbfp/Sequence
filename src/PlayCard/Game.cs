using System;
using System.Collections.Immutable;
using System.Linq;

namespace Sequence.PlayCard
{
    public static class Game
    {
        public static GameEvent PlayCard(GameState state, PlayerHandle player, Card card, Coord coord)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            return PlayCard(state, state.PlayerHandleByIdx.IndexOf(player), card, coord);
        }

        public static GameEvent PlayCard(GameState state, PlayerId player, Card card, Coord coord)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            return PlayCard(state, state.PlayerIdByIdx.IndexOf(player), card, coord);
        }

        private static GameEvent PlayCard(GameState state, int playerIdx, Card card, Coord coord)
        {
            if (card == null)
            {
                throw new ArgumentNullException(nameof(card));
            }

            if (playerIdx == -1)
            {
                throw new PlayCardFailedException(PlayCardError.PlayerIsNotInGame);
            }

            var playerId = state.PlayerIdByIdx[playerIdx];

            if (!playerId.Equals(state.CurrentPlayerId))
            {
                throw new PlayCardFailedException(PlayCardError.PlayerIsNotCurrentPlayer);
            }

            if (!state.PlayerHandByIdx[playerIdx].Contains(card))
            {
                throw new PlayCardFailedException(PlayCardError.PlayerDoesNotHaveCard);
            }

            var chips = state.Chips;
            var deck = ImmutableStack.CreateRange(state.Deck);
            var team = state.PlayerTeamByIdx[playerIdx];

            if (card.IsOneEyedJack())
            {
                if (!chips.ContainsKey(coord))
                {
                    throw new PlayCardFailedException(PlayCardError.CoordIsEmpty);
                }

                if (chips.TryGetValue(coord, out var chip) && chip == team)
                {
                    throw new PlayCardFailedException(PlayCardError.ChipBelongsToPlayerTeam);
                }

                if (state.CoordsInSequence.Contains(coord))
                {
                    throw new PlayCardFailedException(PlayCardError.ChipIsPartOfSequence);
                }

                return new GameEvent
                {
                    ByPlayerId = playerId,
                    CardDrawn = deck.Peek(),
                    CardUsed = card,
                    Chip = null,
                    Coord = coord,
                    Index = state.Version + 1,
                    NextPlayerId = state.PlayerIdByIdx[(playerIdx + 1) % state.NumberOfPlayers],
                };
            }
            else
            {
                if (chips.ContainsKey(coord))
                {
                    throw new PlayCardFailedException(PlayCardError.CoordIsOccupied);
                }

                var board = state.BoardType.Board;

                if (!board.Matches(coord, card))
                {
                    throw new PlayCardFailedException(PlayCardError.CardDoesNotMatchCoord);
                }

                var sequence = board.GetSequence(
                    chips: chips.Add(coord, team),
                    state.CoordsInSequence,
                    coord, team);

                Team? winnerTeam = null;

                if (sequence != null)
                {
                    // Test for win condition:
                    var numSequencesForTeam = state.Sequences
                        .Add(sequence)
                        .Count(seq => seq.Team == team);

                    if (numSequencesForTeam == state.WinCondition)
                    {
                        winnerTeam = team;
                    }
                }

                var nextPlayerId = winnerTeam == null
                    ? state.PlayerIdByIdx[(playerIdx + 1) % state.NumberOfPlayers]
                    : null;

                return new GameEvent
                {
                    ByPlayerId = playerId,
                    CardDrawn = deck.Peek(),
                    CardUsed = card,
                    Chip = team,
                    Coord = coord,
                    Index = state.Version + 1,
                    NextPlayerId = nextPlayerId,
                    Sequence = sequence,
                    Winner = winnerTeam,
                };
            }
        }
    }
}
