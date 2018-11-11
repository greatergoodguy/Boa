using System;
using System.Collections.Generic;
using System.Linq;

public struct AllSnakesState {
    public readonly SnakeState[] all;

    public AllSnakesState(SnakeState[] snakes) {
        this.all = snakes;
    }
}

public struct SnakeState {
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

        Func<SnakeState, bool> StillInGame = (snake) => commands.serverCommands.leftPlayerIds.Contains(snake.ownerId) == false;
        Func<SnakeState, SnakeState> DoTick = (snake) => SnakeReducer.I.DoTick(previousState, snake, commands.playerCommands[snake.ownerId].changeDirection);
        Func<SnakeState, bool> HeadIsNotOnAWall = (snake) => previousState.walls.Contains(snake.headPosition) == false;
        Func<int, SnakeState> NewSnake = (int ownerId) => new SnakeState(new DG_Vector2(0, 0), new Direction(DirectionEnum.Up), true, ownerId, new DG_Vector2[0]);
        IEnumerable<SnakeState> NewSnakes = commands.serverCommands.newPlayerIds.Select(NewSnake);

        var newSnakes = previousSnakes.all
            .Where(StillInGame)
            .Select(DoTick)
            .Where(HeadIsNotOnAWall)
            .Concat(NewSnakes);

        Func<SnakeState, bool> HeadIsNotOnOwnTail = (snake) => snake.tails.Any(x => x == snake.headPosition) == false;

        return new AllSnakesState(
            newSnakes
            .Where(HeadIsNotOnOwnTail)
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

    Direction HandleDirectionChange(Direction previousDirection, DirectionEnum newDirection) {
        if (newDirection == DirectionEnum.None) {
            return previousDirection;
        } else if (
            newDirection == DirectionEnum.Up && previousDirection.direction == DirectionEnum.Down ||
            newDirection == DirectionEnum.Right && previousDirection.direction == DirectionEnum.Left ||
            newDirection == DirectionEnum.Down && previousDirection.direction == DirectionEnum.Up ||
            newDirection == DirectionEnum.Left && previousDirection.direction == DirectionEnum.Right
        ) {
            return previousDirection;
        } else {
            return new Direction(newDirection);
        }
    }
}
