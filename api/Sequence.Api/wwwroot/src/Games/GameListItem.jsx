import React from 'react';
import { Link } from 'react-router-dom';

const time = dt => <time dateTime={dt}>{dt.toLocaleString()}</time>;

class GameListItem extends React.PureComponent {
    render() {
        const { currentPlayer, gameId, lastMoveAt, opponents, userName } = this.props;
        const $opponents = opponents.join(', ');

        let $elements = [];

        if (currentPlayer) {
            $elements.push((
                <div key="opponents">
                    Playing with <strong>{$opponents}</strong>
                </div>
            ));

            if (currentPlayer !== userName) {
                $elements.push((
                    <div key="current-player">
                        It's <strong>{currentPlayer}</strong>'s turn
                    </div>
                ));
            }

            if (lastMoveAt.valueOf() > 0) {
                $elements.push((
                    <div key="last-move-at">
                        <em>Last move at {time(lastMoveAt)}</em>
                    </div>
                ));
            } else {
                $elements.push((
                    <div key="last-move-at">
                        <em>No moves yet</em>
                    </div>
                ))
            }
        } else {
            $elements.push((
                <div key="opponents">
                    Played with <strong>{$opponents}</strong>
                </div>
            ));

            $elements.push((
                <div key="ended-at">
                    <em>Ended at {time(lastMoveAt)}</em>
                </div>
            ));
        }

        return (
            <div className="game-list-item">
                <Link to={`/games/${gameId}`}>
                    {$elements.slice(0, 1)}
                </Link>

                <div className="game-list-item-subtitle">
                    {$elements.slice(1)}
                </div>
            </div>
        );
    }
}

export default GameListItem;
