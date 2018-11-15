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

    public AllApplesState SpawnApples(int tick, IEnumerable<DG_Vector2> snakeHeads) {
        Func<AppleState, bool> NotOnSnakeHead = (apple) => snakeHeads.Contains(apple.position) == false;

        return new AllApplesState(
            this.all.Concat(
                GetNewApples(tick).Where(NotOnSnakeHead)
            ).ToArray(),
            this.eatenApples
        );
    }

    public AllApplesState RemoveEatenApples(AllSnakesState allSnakesState) {
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
        Func<AppleState, bool> InsideWall = (apple) => WallsReducer.IsPositionInsideWalls(apple.position, walls);

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
