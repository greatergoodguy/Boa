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
                        new SnakeState(ownerNetId: 0),
                        new SnakeState(ownerNetId: -1, headPosition: new DG_Vector2(10, -3), direction: new Direction(DirectionEnum.Left)),
                        new SnakeState(ownerNetId: -2, headPosition: new DG_Vector2(10, -4), direction: new Direction(DirectionEnum.Left)),
                        new SnakeState(ownerNetId: -3, headPosition: new DG_Vector2(10, -5), direction: new Direction(DirectionEnum.Left)),
                        new SnakeState(ownerNetId: -4, headPosition: new DG_Vector2(10, -6), direction: new Direction(DirectionEnum.Left)),
                        new SnakeState(ownerNetId: -5, headPosition: new DG_Vector2(10, -7), direction: new Direction(DirectionEnum.Left)),
                        new SnakeState(ownerNetId: -6, headPosition: new DG_Vector2(10, -8), direction: new Direction(DirectionEnum.Left)),
                        new SnakeState(ownerNetId: -7, headPosition: new DG_Vector2(10, -9), direction: new Direction(DirectionEnum.Left)),
                        new SnakeState(ownerNetId: -8, headPosition: new DG_Vector2(10, -10), direction: new Direction(DirectionEnum.Left)),
                        new SnakeState(ownerNetId: -9, headPosition: new DG_Vector2(10, -11), direction: new Direction(DirectionEnum.Left)),
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
