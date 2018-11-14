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
                apples : new AllApplesState(new AppleState[0], new DG_Vector2[0]),
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
                            ownerNetId: 0
                        )
                    }
                ),
                players : new int[] { 0 },
                apples : new AllApplesState(new AppleState[0], new DG_Vector2[0]),
                walls : GetStartingWalls()
            );
        }
    }

    static DG_Vector2[] GetStartingWalls(int size = 50) {
        var walls = new List<DG_Vector2>();

        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                if (x == 0) {
                    walls.Add(new DG_Vector2((-size / 2) - 1, y - (size / 2)));
                }
                if (x == size - 1) {
                    walls.Add(new DG_Vector2(size / 2, y - (size / 2)));
                }
                if (y == 0) {
                    walls.Add(new DG_Vector2(x - (size / 2), (-size / 2) - 1));
                }
                if (y == size - 1) {
                    walls.Add(new DG_Vector2(x - (size / 2), size / 2));
                }
            }
        }

        return walls.ToArray();
    }
}
