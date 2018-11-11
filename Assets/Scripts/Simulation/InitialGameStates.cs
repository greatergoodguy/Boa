using System.Collections.Generic;

public static class InitialGameStates {
    public static GameState ServerInitialGameState {
        get {
            return new GameState(
                tick: 0,
                snakes: new AllSnakesState(
                    new SnakeState[0]
                ),
                players : new int[0],
                apples : new AllApplesState(new AppleState[0]),
                walls : GetStartingWalls()
            );
        }
    }

    public static GameState OfflineInitialGameState {
        get {
            return new GameState(
                tick: 0,
                snakes: new AllSnakesState(
                    new SnakeState[] {
                        new SnakeState(
                            position: DG_Vector2.zero,
                            direction: new Direction(DirectionEnum.Up),
                            isAlive: true,
                            ownerNetId: 0,
                            tails: new DG_Vector2[0]
                        )
                    }
                ),
                players : new int[] { 0 },
                apples : new AllApplesState(new AppleState[0]),
                walls : GetStartingWalls()
            );
        }
    }

    static DG_Vector2[] GetStartingWalls(int size = 100) {
        var walls = new List<DG_Vector2>();

        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                if (x == 0) {
                    walls.Add(new DG_Vector2(-1, y));
                }
                if (x == size - 1) {
                    walls.Add(new DG_Vector2(size, y));
                }
                if (y == 0) {
                    walls.Add(new DG_Vector2(x, -1));
                }
                if (y == size - 1) {
                    walls.Add(new DG_Vector2(x, size));
                }
            }
        }

        return walls.ToArray();
    }
}
