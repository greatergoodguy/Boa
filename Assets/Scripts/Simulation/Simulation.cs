using System.Collections.Generic;
using System.Linq;

public class Simulation {
    public int tick { get; private set; }

    PreviousGameStates previousGameStates = new PreviousGameStates();

    public Simulation(GameState initialState) {
        tick = 0;
        previousGameStates[0] = initialState;
    }

    public void LoadGameState(int tick, GameState gameState) {
        this.tick = tick;
        previousGameStates.Clear();
        previousGameStates[this.tick] = gameState;
    }

    public GameState DoTick(PlayerCommands commands) {
        // Toolbox.Log($"Simulation DoTick {tick} -> {tick + 1}");
        tick++;
        previousGameStates[tick] = (GameState) GameStateReducer.I.DoTick(tick, previousGameStates[tick - 1], commands);
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
    public readonly int tick;
    public readonly AllSnakesState snakes;
    public readonly int[] players;
    public readonly AllApplesState apples;

    public GameState(int tick, AllSnakesState snakes, int[] players, AllApplesState apples) {
        this.tick = tick;
        this.snakes = snakes;
        this.players = players;
        this.apples = apples;
    }
}

struct GameStateReducer {
    public static GameStateReducer I;

    public GameState DoTick(int tick, GameState previousState, PlayerCommands commands) {
        return new GameState(
            tick: tick,
            snakes: AllSnakesReducer.I.DoTick(previousState, commands),
            players : PlayersReducer.DoTick(previousState, commands),
            apples : AllApplesReducer.I.DoTick(previousState, commands)
        );
    }
}

static class PlayersReducer {
    public static int[] DoTick(GameState previousState, PlayerCommands commands) {
        var newState = new HashSet<int>(previousState.players);
        newState.UnionWith(commands.serverCommands.newPlayerIds);
        newState.ExceptWith(commands.serverCommands.leftPlayerIds);
        return newState.ToArray();
    }
}
