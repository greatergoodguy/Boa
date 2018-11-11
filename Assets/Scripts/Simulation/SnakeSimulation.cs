using System.Collections.Generic;
using System.Linq;

public struct AllSnakesState : IGameState {
    public readonly SnakeState[] all;

    public AllSnakesState(SnakeState[] snakes) {
        this.all = snakes;
    }
}

public struct SnakeState : IGameState {
    public readonly DG_Vector2 headPosition;
    public readonly Direction direction;
    public readonly bool isAlive;
    public readonly int ownerId;
    public readonly DG_Vector2[] tails;

    public SnakeState(DG_Vector2 position, Direction direction, bool isAlive, int ownerNetId, DG_Vector2[] tails) {
        this.headPosition = position;
        this.direction = direction;
        this.isAlive = isAlive;
        this.ownerId = ownerNetId;
        this.tails = tails;
    }
}

public struct AllSnakesReducer {
    public static AllSnakesReducer I;

    public AllSnakesState DoTick(GameState previousState, PlayerCommands commands) {
        var previousSnakes = previousState.snakes;

        return new AllSnakesState(
            previousSnakes.all
            .Select(x => SnakeReducer.I.DoTick(previousState, x, commands.playerCommands[x.ownerId].changeDirection))
            .Concat(commands.serverCommands.newPlayerIds.Select(x => new SnakeState(new DG_Vector2(0, 0), new Direction(DirectionEnum.Up), true, x, new DG_Vector2[0])))
            .ToArray()
        );
    }
}

public struct SnakeReducer {
    public static SnakeReducer I;

    public SnakeState DoTick(GameState previousGameState, SnakeState previousSnakeState, DirectionEnum changeDirectionCommand) {
        var previousSnake = previousSnakeState;

        var newTails = previousSnake.tails.ToArray();

        if (IsHeadOnApple(previousSnake.headPosition, previousGameState.apples)) {
            newTails = newTails.Append(new DG_Vector2()).ToArray();
        }

        // Change direction
        var newDirection = HandleDirectionChange(previousSnake.direction, changeDirectionCommand);

        // Move
        var newHeadPosition = previousSnake.headPosition + newDirection.GetMoveVector();
        if (newTails.Length > 0) {
            newTails = newTails.Skip(newTails.Length - 1).Concat(newTails.Take(newTails.Length - 1)).ToArray();
            newTails[0] = previousSnake.headPosition;
        }

        return new SnakeState(
            newHeadPosition,
            newDirection,
            previousSnake.isAlive,
            previousSnake.ownerId,
            newTails
        );
    }

    bool IsHeadOnApple(DG_Vector2 headPosition, AllApplesState applesState) {
        return applesState.all.Any(x => x.position == headPosition);
    }

    Direction HandleDirectionChange(Direction currentDirection, DirectionEnum newDirection) {
        if (newDirection == DirectionEnum.None) {
            return currentDirection;
        } else {
            return new Direction(newDirection);
        }
    }
}
