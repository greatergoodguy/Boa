using System.Linq;

public struct AllApplesState {
    public readonly AppleState[] all;

    public AllApplesState(AppleState[] apples) {
        this.all = apples;
    }
}

public struct AppleState {
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
