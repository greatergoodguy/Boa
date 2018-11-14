using System;
using System.Collections.Generic;
using System.Linq;

public struct AllApplesState {
    public readonly AppleState[] all;
    public readonly DG_Vector2[] eatenApples;

    public AllApplesState(AppleState[] apples, DG_Vector2[] eatenApples) {
        this.all = apples;
        this.eatenApples = eatenApples;
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

    public AllApplesState FirstPass(GameState previousState, PlayerCommands commands) {
        return new AllApplesState(
            previousState.apples.all
            .Concat(GetNewApples(previousState.tick))
            .ToArray(),
            previousState.apples.eatenApples
        );
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

    public AllApplesState SecondPass(GameState firstPassResult, PlayerCommands commands) {
        var secondPassApples = firstPassResult.apples.all.ToList();
        var eatenApples = new List<DG_Vector2>();

        foreach (AppleState apple in firstPassResult.apples.all) {
            if (IsAppleOnSnakeHead(apple.position, firstPassResult.snakes)) {
                secondPassApples.Remove(apple);
                eatenApples.Add(apple.position);
            }
        }

        int topWall = firstPassResult.walls.Max(x => x.y);
        int rightWall = firstPassResult.walls.Max(x => x.x);
        int bottomWall = firstPassResult.walls.Min(x => x.y);
        int leftWall = firstPassResult.walls.Min(x => x.x);

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
            secondPassApples.Where(InsideWall).ToArray(),
            eatenApples.ToArray()
        );
    }

    bool IsAppleOnSnakeHead(DG_Vector2 applePosition, AllSnakesState allSnakesState) {
        return allSnakesState.all.Any(x => x.headPosition == applePosition);
    }
}
