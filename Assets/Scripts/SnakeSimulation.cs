using System.Collections.Generic;
using System.Linq;

public struct AllSnakesState : IGameState {
    public readonly SnakeState[] all;

    public AllSnakesState(SnakeState[] snakes) {
        this.all = snakes;
    }
}

public struct SnakeState : IGameState {
    public readonly DG_Vector2 position;
    public readonly Direction direction;
    public readonly bool isAlive;
    public readonly int ownerNetId;

    public SnakeState(DG_Vector2 position, Direction direction, bool isAlive, int ownerNetId) {
        this.position = position;
        this.direction = direction;
        this.isAlive = isAlive;
        this.ownerNetId = ownerNetId;
    }
}

public struct AllSnakesReducer {
    public static AllSnakesReducer I;

    public AllSnakesState DoTick(GameState previousState, PlayerCommands commands) {
        var previousSnakes = previousState.snakes;

        return new AllSnakesState(
            previousSnakes.all.Select(x => SnakeReducer.I.DoTick(x, commands[x.ownerNetId].changeDirection)).ToArray()
        );
    }
}

public struct SnakeReducer {
    public static SnakeReducer I;

    public SnakeState DoTick(SnakeState previousState, DirectionEnum changeDirectionCommand) {
        var previousSnake = previousState;

        // Change direction
        var newDirection = HandleDirectionChange(previousSnake.direction, changeDirectionCommand);
        // Move
        var newPosition = previousSnake.position + newDirection.GetMoveVector();

        return new SnakeState(
            newPosition,
            newDirection,
            previousSnake.isAlive,
            previousSnake.ownerNetId
        );
    }

    Direction HandleDirectionChange(Direction currentDirection, DirectionEnum newDirection) {
        if (newDirection == DirectionEnum.None) {
            return currentDirection;
        } else {
            return new Direction(newDirection);
        }
    }
}
