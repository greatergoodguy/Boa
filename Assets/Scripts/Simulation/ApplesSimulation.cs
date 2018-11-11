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
        var newApples = previousApples.all.ToList(); 
        foreach(AppleState apple in previousApples.all) {
            if(IsOnAppleOnHead(apple.position, previousState.snakes)) {
                newApples.Remove(apple);
            }
        }

        return new AllApplesState(
            newApples
            .Concat(GetNewApples(previousState.tick))
            .ToArray()
        );
    }

    bool IsOnAppleOnHead(DG_Vector2 applePosition, AllSnakesState allSnakesState)
    {
        return allSnakesState.all.Any(x => x.headPosition == applePosition);
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
