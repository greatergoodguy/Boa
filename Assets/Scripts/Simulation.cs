using System.Collections.Generic;
using UnityEngine;

public class Simulation {
    public int tick { get; private set; }

    PreviousGameStates previousGameStates = new PreviousGameStates();

    public Simulation() {
        // Initial GameState
        tick = 0;
        previousGameStates[tick] = new GameState(
            new AllSnakesState(
                new SnakeState[] {
                    new SnakeState(new DG_Vector2(0, 0), new Direction(DirectionEnum.Up), true),
                    new SnakeState(new DG_Vector2(3, -4), new Direction(DirectionEnum.Up), true),
                }
            )
        );
    }

    public GameState DoTick(Commands commands) {
        Toolbox.Log($"Simulation DoTick {tick} -> {tick + 1}");
        tick++;
        previousGameStates[tick] = (GameState) GameStateReducer.I.DoTick(previousGameStates[tick - 1], commands);
        return previousGameStates[tick];
    }

    public GameState RollbackToTick(int tick) {
        Toolbox.Log($"Simulation RollbackToTick {this.tick} -> {tick}");
        this.tick = tick;
        return previousGameStates[tick];
    }
}

class PreviousGameStates : Dictionary<int, GameState> { }

public interface IGameState { }

public struct GameState : IGameState {
    public readonly AllSnakesState snakes;

    public GameState(AllSnakesState snakes) {
        this.snakes = snakes;
    }
}

public interface IReducer<T, U> where T : IGameState where U : IGameState {
    U DoTick(T previousState, Commands commands);
}

struct GameStateReducer : IReducer<GameState, GameState> {
    public static GameStateReducer I;

    public GameState DoTick(GameState previousState, Commands commands) {
        return new GameState(
            (AllSnakesState) AllSnakesReducer.I.DoTick(previousState, commands)
        );
    }
}
