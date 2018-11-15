using System;
using System.Collections.Generic;
using System.Linq;

public struct AllApplesState {
    const int spawnWidth = 50;
    const int ticksPerSpawn = 1;

    public readonly AppleState[] all;
    public readonly DG_Vector2[] eatenApples;

    public AllApplesState(AppleState[] apples, DG_Vector2[] eatenApples) {
        this.all = apples;
        this.eatenApples = eatenApples;
    }

    public AllApplesState SpawnApples(int tick) {
        return new AllApplesState(
            this.all.Concat(GetNewApples(tick)).ToArray(),
            this.eatenApples
        );
    }

    public AllApplesState EatApples(AllSnakesState allSnakesState) {
        var secondPassApples = this.all.ToList();
        var eatenApples = new List<DG_Vector2>();

        foreach (AppleState apple in this.all) {
            if (IsAppleOnSnakeHead(apple.position, allSnakesState)) {
                secondPassApples.Remove(apple);
                eatenApples.Add(apple.position);
            }
        }
        return new AllApplesState(
            secondPassApples.ToArray(),
            eatenApples.ToArray()
        );
    }

    public AllApplesState RemoveWhereOnOrOutsideWalls(DG_Vector2[] walls) {
        int topWall = walls.Max(x => x.y);
        int rightWall = walls.Max(x => x.x);
        int bottomWall = walls.Min(x => x.y);
        int leftWall = walls.Min(x => x.x);

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
            this.all.Where(InsideWall).ToArray(),
            this.eatenApples
        );
    }

    static AppleState[] GetNewApples(int previousTick) {
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

    static bool IsAppleOnSnakeHead(DG_Vector2 applePosition, AllSnakesState allSnakesState) {
        return allSnakesState.all.Any(x => x.headPosition == applePosition);
    }
}

public struct AppleState {
    public readonly DG_Vector2 position;

    public AppleState(DG_Vector2 position) {
        this.position = position;
    }
}
