using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class Simulation {
    public int tick { get; private set; }

    Dictionary<int, GameState> previousGameStates = new Dictionary<int, GameState>();

    public Simulation(GameState initialState, int tick = 0) {
        previousGameStates[tick] = initialState;
        this.tick = tick;
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

public struct GameState {
    public readonly int tick;
    public readonly AllSnakesState snakes;
    public readonly int[] players;
    public readonly AllApplesState apples;
    public readonly DG_Vector2[] walls;

    public GameState(int tick, AllSnakesState snakes, int[] players, AllApplesState apples, DG_Vector2[] walls) {
        this.tick = tick;
        this.snakes = snakes;
        this.players = players;
        this.apples = apples;
        this.walls = walls;
    }

    public string Serialize() {
        return JsonConvert.SerializeObject(this);
    }
}

struct GameStateReducer {
    public static GameStateReducer I;

    public GameState DoTick(int tick, GameState previousState, PlayerCommands commands) {
        return new GameState(
            tick: tick,
            snakes: AllSnakesReducer.I.DoTick(previousState, commands),
            players: PlayersReducer.DoTick(previousState, commands),
            apples: AllApplesReducer.I.DoTick(previousState, commands),
            walls: WallsReducer.DoTick(previousState)
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

static class WallsReducer {
    const int ticksPerShrink = 1;

    public static DG_Vector2[] DoTick(GameState previousGameState) {
        if (previousGameState.tick % ticksPerShrink != 0) return previousGameState.walls;

        return previousGameState.walls.Select(x => TowardsCenter(x)).ToArray();
    }

    static DG_Vector2 TowardsCenter(DG_Vector2 vector2) {
        return new DG_Vector2(vector2.x + (vector2.x < 0 ? 1 : -1), vector2.y + (vector2.y < 0 ? 1 : -1));
    }
}
