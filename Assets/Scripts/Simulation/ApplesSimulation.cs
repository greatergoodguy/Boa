using System;
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

    const int spawnWidth = 50;
    const int ticksPerSpawn = 1;

    public AllApplesState DoTick(GameState previousState, PlayerCommands commands) {
        var previousApples = previousState.apples;

        var newApples = previousApples.all.ToList();

        foreach (AppleState apple in previousApples.all) {
            if (IsOnAppleOnHead(apple.position, previousState.snakes)) {
                newApples.Remove(apple);
            }
        }

        int topWall = previousState.walls.Max(x => x.y);
        int rightWall = previousState.walls.Max(x => x.x);
        int bottomWall = previousState.walls.Min(x => x.y);
        int leftWall = previousState.walls.Min(x => x.x);

        Func<AppleState, bool> InsideWall = (apple) => {
            if (
                apple.position.y >= topWall ||
                apple.position.y <= bottomWall ||
                apple.position.x >= rightWall ||
                apple.position.x <= leftWall
            ) return false;

            return true;
        };

        return new AllApplesState(
            newApples
            .Concat(GetNewApples(previousState.tick))
            .Where(InsideWall)
            .ToArray()
        );
    }

    bool IsOnAppleOnHead(DG_Vector2 applePosition, AllSnakesState allSnakesState) {
        return allSnakesState.all.Any(x => x.headPosition == applePosition);
    }

    AppleState[] GetNewApples(int previousTick) {
        if (previousTick % ticksPerSpawn != 0) return new AppleState[0];

        var random = new System.Random(previousTick);

        return new AppleState[] {
            new AppleState(
                new DG_Vector2(
                    random.Next(spawnWidth) - spawnWidth / 2,
                    random.Next(spawnWidth) - spawnWidth / 2
                )
            )
        };
    }
}
