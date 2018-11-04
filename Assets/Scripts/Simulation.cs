using System.Collections.Generic;
using UnityEngine;

public class Simulation {
    public int tick { get; private set; }
    public GameState gameState { get; private set; }

    PreviousGameStates previousGameState = new PreviousGameStates();

    public Simulation() {
        gameState = new GameState() {
            snake = new SnakeState(new DG_Vector2(0, 0), new Direction(DirectionEnum.Up), true)
        };
        previousGameState[tick] = gameState;
    }

    public GameState DoTick(Commands commands) {
        Toolbox.Log($"Simulation DoTick {tick} -> {tick + 1}");
        tick++;
        gameState = Simulator.DoTick(gameState, commands);
        previousGameState[tick] = gameState;
        return gameState;
    }

    public GameState RollbackToTick(int tick) {
        Toolbox.Log($"Simulation RollbackToTick {this.tick} -> {tick}");
        this.tick = tick;
        gameState = previousGameState[tick];
        return gameState;
    }
}

class PreviousGameStates : Dictionary<int, GameState> { }

static class Simulator {
    public static GameState DoTick(GameState previousState, Commands commands) {
        var newState = previousState;
        newState.snake = (SnakeState) SnakeReducer.I.DoTick(previousState, commands);
        return newState;
    }
}

public interface IGameState { }

public struct GameState : IGameState {
    public SnakeState snake;
}

public interface IReducer {
    IGameState DoTick(GameState previousState, Commands commands);
}

public struct SnakeState : IGameState {
    public readonly DG_Vector2 position;
    public readonly Direction direction;
    public readonly bool isAlive;

    public SnakeState(DG_Vector2 position, Direction direction, bool isAlive) {
        this.position = position;
        this.direction = direction;
        this.isAlive = isAlive;
    }
}

public struct SnakeReducer : IReducer {
    public static SnakeReducer I;

    public IGameState DoTick(GameState previousState, Commands commands) {
        var previousSnake = previousState.snake;

        // Change direction
        var newDirection = HandleDirectionChange(previousSnake.direction, commands);
        // Move
        var newPosition = previousSnake.position + newDirection.GetMoveVector();

        return new SnakeState(
            newPosition,
            newDirection,
            previousSnake.isAlive
        );
    }

    Direction HandleDirectionChange(Direction currentDirection, Commands commands) {
        if (commands.changeDirection == DirectionEnum.None) {
            return currentDirection;
        } else {
            return new Direction(commands.changeDirection);
        }
    }
}
