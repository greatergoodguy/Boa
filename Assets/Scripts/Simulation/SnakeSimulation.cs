using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct AllSnakesState {
    public readonly SnakeState[] all;

    public AllSnakesState(SnakeState[] snakes) {
        this.all = snakes;
    }

    public AllSnakesState RemoveIfOwnerLeftTheGame(PlayerCommands commands) {
        return new AllSnakesState(
            this.all.Where(snake => commands.serverCommands.leftPlayerIds.Contains(snake.ownerId) == false).ToArray()
        );
    }

    public AllSnakesState ForEachSnake(Func<SnakeState, SnakeState> foo) {
        return new AllSnakesState(
            this.all.Select(foo).ToArray()
        );
    }

    public AllSnakesState RemoveIfOutsideWalls(DG_Vector2[] walls) {
        return new AllSnakesState(
            this.all.Where(snake => WallsReducer.IsPositionInsideWalls(snake.headPosition, walls)).ToArray()
        );
    }

    public AllSnakesState AddSnakesForNewPlayers(PlayerCommands commands) {
        Func<int, SnakeState> NewSnake = (int ownerId) => new SnakeState(ownerId);
        var newSnakes = commands.serverCommands.newPlayerIds.Select(NewSnake);

        return new AllSnakesState(
            this.all.Concat(newSnakes).ToArray()
        );
    }

    public AllSnakesState RemoveIfOnOwnTail() {
        return new AllSnakesState(
            this.all.Where(snake => snake.tails.Any(x => x == snake.headPosition) == false).ToArray()
        );
    }

    public AllSnakesState RemoveIfOnOtherSnakesTail() {
        var allSnakes = this.all;
        return new AllSnakesState(
            this.all.Where((snake) => allSnakes
                .Where(x => x.ownerId != snake.ownerId)
                .Any(x => x.tails.Any(y => y == snake.headPosition)) == false
            )
            .ToArray()
        );
    }

    public AllSnakesState RemoveIfHeadIsOnOtherSnakesHead() {
        var allSnakes = this.all;
        return new AllSnakesState(
            this.all.Where((snake) => allSnakes
                .Where(x => x.ownerId != snake.ownerId)
                .Any(x => x.headPosition == snake.headPosition) == false
            )
            .ToArray()
        );
    }
}

public struct SnakeState {
    public readonly DG_Vector2 headPosition;
    public readonly Direction direction;
    public readonly bool isAlive;
    public readonly int ownerId;
    public readonly DG_Vector2[] tails;
    public readonly bool ateAppleLastTick;

    public SnakeState(int ownerNetId) {
        this.headPosition = DG_Vector2.zero;
        this.direction = new Direction(DirectionEnum.Up);
        this.isAlive = true;
        this.ownerId = ownerNetId;
        this.tails = new DG_Vector2[0];
        this.ateAppleLastTick = false;
    }

    public SnakeState(SnakeState snakeToCopy, DG_Vector2? headPosition = null, Direction? direction = null, bool? isAlive = null, int? ownerId = null, DG_Vector2[] tails = null, bool? ateAppleLastTick = null) {
        this.headPosition = headPosition.HasValue ? headPosition.Value : snakeToCopy.headPosition;
        this.direction = direction.HasValue ? direction.Value : snakeToCopy.direction;
        this.isAlive = isAlive.HasValue ? isAlive.Value : snakeToCopy.isAlive;
        this.ownerId = ownerId.HasValue ? ownerId.Value : snakeToCopy.ownerId;
        this.tails = tails != null ? tails : snakeToCopy.tails;
        this.ateAppleLastTick = ateAppleLastTick.HasValue ? ateAppleLastTick.Value : snakeToCopy.ateAppleLastTick;
    }

    public SnakeState(DG_Vector2 headPosition, Direction direction, int ownerNetId, DG_Vector2[] tails = null, bool ateAppleLastTick = false, bool isAlive = true) {
        this.headPosition = headPosition;
        this.direction = direction;
        this.isAlive = isAlive;
        this.ownerId = ownerNetId;
        this.tails = tails == null ? new DG_Vector2[0] : tails;
        this.ateAppleLastTick = ateAppleLastTick;
    }

    public SnakeState AddTails() {
        return new SnakeState(
            this,
            tails : this.ateAppleLastTick ? this.tails.Append(new DG_Vector2()).ToArray() : this.tails
        );
    }

    public SnakeState ChangeDirection(int tick, PlayerCommands commands) {
        DirectionEnum changeDirectionCommand;
        if (this.ownerId >= 0) {
            changeDirectionCommand = commands.playerCommands[this.ownerId].changeDirection;
        } else {
            changeDirectionCommand = Direction.Random((this.ownerId * 100) + tick);
        }
        return new SnakeState(
            this,
            direction : HandleDirectionChange(this.direction, changeDirectionCommand)
        );
    }

    public SnakeState MoveHead() {
        return new SnakeState(
            this,
            headPosition : this.headPosition + this.direction.GetMoveVector()
        );
    }

    public SnakeState MoveTails() {
        var newTails = this.tails;
        if (this.tails.Length > 0) {
            newTails = newTails.Skip(newTails.Length - 1).Concat(newTails.Take(newTails.Length - 1)).ToArray();
            newTails[0] = this.headPosition;
        }
        return new SnakeState(
            this,
            tails : newTails
        );
    }

    public SnakeState EatAppleCheck(AllApplesState apples) {
        return new SnakeState(
            this,
            ateAppleLastTick : IsHeadOnApple(this.headPosition, apples)
        );
    }

    static Direction HandleDirectionChange(Direction previousDirection, DirectionEnum newDirection) {
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

    static bool IsHeadOnApple(DG_Vector2 headPosition, AllApplesState applesState) {
        return applesState.all.Any(x => x.position == headPosition);
    }
}
