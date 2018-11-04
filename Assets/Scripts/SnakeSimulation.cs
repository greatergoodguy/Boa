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

    public SnakeState(DG_Vector2 position, Direction direction, bool isAlive) {
        this.position = position;
        this.direction = direction;
        this.isAlive = isAlive;
    }
}

public struct AllSnakesReducer : IReducer<GameState, AllSnakesState> {
    public static AllSnakesReducer I;

    public AllSnakesState DoTick(GameState previousState, Commands commands) {
        var previousSnakes = previousState.snakes;

        return new AllSnakesState(
            previousSnakes.all.Select(x => SnakeReducer.I.DoTick(x, commands)).ToArray()
        );
    }

    Direction HandleDirectionChange(Direction currentDirection, Commands commands) {
        if (commands.changeDirection == DirectionEnum.None) {
            return currentDirection;
        } else {
            return new Direction(commands.changeDirection);
        }
    }
}

public struct SnakeReducer : IReducer<SnakeState, SnakeState> {
    public static SnakeReducer I;

    public SnakeState DoTick(SnakeState previousState, Commands commands) {
        var previousSnake = previousState;

        // Change direction
        var newDirection = HandleDirectionChange(previousSnake.direction, commands);
        // Move
        var newPosition = previousSnake.position + newDirection.GetMoveVector();

        return new SnakeState(
            newPosition,
            newDirection,
            previousSnake.isAlive
        );
    }

    Direction HandleDirectionChange(Direction currentDirection, Commands commands) {
        if (commands.changeDirection == DirectionEnum.None) {
            return currentDirection;
        } else {
            return new Direction(commands.changeDirection);
        }
    }
}
