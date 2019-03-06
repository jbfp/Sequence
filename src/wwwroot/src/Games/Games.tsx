import React, { useContext, useEffect, useRef, useState } from 'react';
import { ServerContext } from '../contexts';
import { Game, GameCollections } from './types';
import GamesView from './GamesView';

export default function Games() {
    const context = useContext(ServerContext);
    const [games, setGames] = useState<GameCollections | null>(null);
    const intervalId = useRef<number | undefined>(undefined);

    async function loadGamesAsync() {
        const allGames = await context.getGamesAsync();
        const value = mapAllGamesToCollection(allGames, context.userName);
        setGames(value);
    }

    useEffect(() => { loadGamesAsync(); }, [context]);

    useEffect(() => {
        intervalId.current = window.setInterval(() => loadGamesAsync(), 10000);
        return () => window.clearInterval(intervalId.current);
    }, [context]);

    if (context && games) {
        return <GamesView games={games} userName={context.userName} />
    }

    return (
        <div>
            <p>Loading games...</p>
        </div>
    );
}

function mapAllGamesToCollection(allGames: Game[], userName: string): GameCollections {
    const completedGames: Game[] = [];
    const yourTurn: Game[] = [];
    const theirTurn: Game[] = [];

    allGames.map(game => {
        return {
            ...game,
            lastMoveAt: new Date(game.lastMoveAt)
        };
    }).forEach(game => {
        if (game.currentPlayer) {
            if (game.currentPlayer === userName) {
                yourTurn.push(game)
            } else {
                theirTurn.push(game);
            }
        } else {
            completedGames.push(game);
        }
    });

    // Order "your turn" games by oldest. Other people might be waiting for you.
    yourTurn.sort((a, b) => {
        // Games with no moves have a 'last move' value of 0 which will be ordered in the wrong
        // order so, in that case, flip the sort order.
        if (a.lastMoveAt.valueOf() === 0) {
            return 1;
        } else if (b.lastMoveAt.valueOf() === 0) {
            return -1;
        }

        return a.lastMoveAt.valueOf() - b.lastMoveAt.valueOf();
    });

    // Order "their turn" games by time of last move descending.
    theirTurn.sort((a, b) => b.lastMoveAt.valueOf() - a.lastMoveAt.valueOf());

    // Order completed games by time of completion descending.
    completedGames.sort((a, b) => b.lastMoveAt.valueOf() - a.lastMoveAt.valueOf());

    return { completedGames, theirTurn, yourTurn };
}