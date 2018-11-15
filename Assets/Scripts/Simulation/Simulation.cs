using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

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
        previousGameStates[tick] = (GameState) GameStateReducer.DoTick(tick, previousGameStates[tick - 1], commands);
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

static class GameStateReducer {
    public static GameState DoTick(int tick, GameState previousState, PlayerCommands commands) {
        try {
            var newWallsState = WallsReducer.MoveWalls(previousState);

            var newAllSnakesState = previousState.snakes
                .RemoveIfOwnerLeftTheGame(commands)
                .ForEachSnake((snake) => {
                    return snake
                        .AddTails()
                        .ChangeDirection(tick, commands)
                        .MoveTails()
                        .MoveHead()
                        .EatAppleCheck(previousState.apples);
                })
                .RemoveIfOutsideWalls(newWallsState)
                .AddSnakesForNewPlayers(commands)
                .RemoveIfOnOwnTail()
                .RemoveIfOnOtherSnakesTail()
                .RemoveIfHeadIsOnOtherSnakesHead();

            var newAllApplesState = previousState.apples
                .SpawnApples(tick, newAllSnakesState.all.Select(x => x.headPosition))
                .RemoveEatenApples(newAllSnakesState)
                .RemoveWhereOnOrOutsideWalls(newWallsState);

            return new GameState(
                tick: tick,
                snakes: newAllSnakesState,
                players: PlayersReducer.FirstPass(previousState, commands),
                apples: newAllApplesState,
                walls: newWallsState
            );
        } catch (Exception) {
            Debug.LogError($"tick: {tick} | previousState: {previousState.Serialize()} | commands: {commands.Serialize()}");
            throw;
        }
    }
}

static class PlayersReducer {
    public static int[] FirstPass(GameState previousState, PlayerCommands commands) {
        var newState = new HashSet<int>(previousState.players);
        newState.UnionWith(commands.serverCommands.newPlayerIds);
        newState.ExceptWith(commands.serverCommands.leftPlayerIds);
        return newState.ToArray();
    }
}

static class WallsReducer {
    const int ticksPerShrink = 10;

    public static DG_Vector2[] MoveWalls(GameState previousGameState) {
        if (previousGameState.tick % ticksPerShrink != 0) return previousGameState.walls;

        return previousGameState.walls.Select(x => TowardsCenter(x)).ToArray();
    }

    static DG_Vector2 TowardsCenter(DG_Vector2 vector2) {
        return new DG_Vector2(vector2.x + (vector2.x < 0 ? 1 : -1), vector2.y + (vector2.y < 0 ? 1 : -1));
    }

    public static bool IsPositionInsideWalls(DG_Vector2 position, DG_Vector2[] walls) {
        int topWall = walls.Max(x => x.y);
        int rightWall = walls.Max(x => x.x);
        int bottomWall = walls.Min(x => x.y);
        int leftWall = walls.Min(x => x.x);

        if (
            position.y >= topWall ||
            position.y <= bottomWall ||
            position.x >= rightWall ||
            position.x <= leftWall
        ) {
            return false;
        } else {
            return true;
        }
    }
}
