public static class InitialGameStates {
    public static readonly GameState ServerInitialGameState = new GameState(
        tick: 0,
        snakes: new AllSnakesState(
            new SnakeState[0]
        ),
        players : new int[0],
        apples : new AllApplesState(new AppleState[0])
    );

    public static readonly GameState OfflineInitialGameState = new GameState(
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
        apples : new AllApplesState(new AppleState[0])
    );
}
