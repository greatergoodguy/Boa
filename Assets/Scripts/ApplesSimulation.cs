using System.Linq;

public struct AllApplesState : IGameState {
    public readonly AppleState[] all;

    public AllApplesState(AppleState[] apples) {
        this.all = apples;
    }
}

public struct AppleState : IGameState {
    public readonly DG_Vector2 position;

    public AppleState(DG_Vector2 position) {
        this.position = position;
    }
}

public struct AllApplesReducer {
    public static AllApplesReducer I;

    public AllApplesState DoTick(GameState previousState, PlayerCommands commands) {
        var previousApples = previousState.apples;

        return new AllApplesState(
            previousApples.all
            .Concat(GetNewApples(previousState.tick))
            .ToArray()
        );
    }

    AppleState[] GetNewApples(int previousTick) {
        return new AppleState[] {
            new AppleState(new DG_Vector2(previousTick, previousTick))
        };
    }
}

// public struct AppleReducer {
//     public static AppleReducer I;

//     public AppleState DoTick(AppleState previousState, DirectionEnum changeDirectionCommand) {
//         var previousSnake = previousState;

//         // Change direction
//         var newDirection = HandleDirectionChange(previousSnake.direction, changeDirectionCommand);
//         // Move
//         var newPosition = previousSnake.position + newDirection.GetMoveVector();

//         return new AppleState(
//             newPosition,
//             newDirection,
//             previousSnake.isAlive,
//             previousSnake.ownerNetId
//         );
//     }

//     Direction HandleDirectionChange(Direction currentDirection, DirectionEnum newDirection) {
//         if (newDirection == DirectionEnum.None) {
//             return currentDirection;
//         } else {
//             return new Direction(newDirection);
//         }
//     }
// }
