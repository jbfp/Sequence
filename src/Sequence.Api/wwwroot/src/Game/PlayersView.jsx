import React from 'react';

class PlayersView extends React.PureComponent {
    render() {
        const { players } = this.props;

        const $players = players.map((player, i) => {
            return (
                <div
                    key={i}
                    className="players-player"
                    data-team={player.team}
                    data-current-player={player.isCurrentPlayer}
                >
                    <div>
                        {player.id}
                    </div>
                </div>
            );
        });

        return (
            <div className="players">
                {$players}
            </div>
        );
    }
}

export default PlayersView;
