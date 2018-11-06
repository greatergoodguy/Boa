using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class Scheduler : MonoBehaviour {
    public static Scheduler I;

    public Presenter presenter;
    float ticksPerSecond = 2;

    Simulation simulation = new Simulation();
    CommandHistory commandHistory = new CommandHistory();

    bool manualTickDebugMode;
    float elapsedTime = 0;
    int safeTick = 0;
    GameState safeGameState;
    bool running = false;

    void Awake() {
        I = this;
    }

    public RequestGameStateMessage SerializeGameStateAndCommands() {
        return new RequestGameStateMessage() {
            commandHistory = commandHistory,
            gameState = safeGameState,
            safeTick = safeTick,
        };
    }

    public void LoadGameStateAndCommands(RequestGameStateMessage gameStateMessage) {
        safeGameState = gameStateMessage.gameState;
        safeTick = gameStateMessage.safeTick;
        commandHistory = gameStateMessage.commandHistory;
        simulation.LoadGameState(safeTick, safeGameState);
        Go();
    }

    public void GoWithDefaultGameState() {
        Debug.Log("GoWithDefaultGameState");
        safeGameState = simulation.GetInitialGameState();
        safeTick = simulation.tick;
        Go();
    }

    public void Go() {
        Debug.Log("Go");
        Present(safeGameState);
        running = true;
    }

    public void Stop() {
        running = false;
    }

    void Update() {
        if (!running) return;

        if (NetworkManager.singleton.client != null) CheckLocalPlayerInput();

        if (DG_Input.ToggleManualTick()) {
            manualTickDebugMode = !manualTickDebugMode;
        }

        if (manualTickDebugMode) {
            DoManualTickStuff();
        } else {
            DoNormalTickStuff();
        }
    }

    void CheckLocalPlayerInput() {
        if (DG_Input.GoLeft()) {
            commandHistory[simulation.tick + 1] = new Commands(DirectionEnum.Left);
        }
        if (DG_Input.GoUp()) {
            commandHistory[simulation.tick + 1] = new Commands(DirectionEnum.Up);
        }
        if (DG_Input.GoRight()) {
            commandHistory[simulation.tick + 1] = new Commands(DirectionEnum.Right);
        }
        if (DG_Input.GoDown()) {
            commandHistory[simulation.tick + 1] = new Commands(DirectionEnum.Down);
        }
    }

    void DoManualTickStuff() {
        if (simulation.tick != safeTick && DG_Input.NextTick()) {
            if (DG_Input.NextTickBig()) {
                RollForwardToTick(Mathf.Min(simulation.tick + 10, safeTick));
            } else {
                DoTick();
            }
        } else if (DG_Input.BackTick() && simulation.tick > 0) {
            if (DG_Input.BackTickBig()) {
                RollbackToTick(Mathf.Max(simulation.tick - 10, 0));
            } else {
                RollbackToTick(simulation.tick - 1);
            }
        }
    }

    void DoNormalTickStuff() {
        elapsedTime += Time.deltaTime;

        if (elapsedTime > 1 / ticksPerSecond) {
            if (HaveAllCommandsForNextTick() == false) {
                commandHistory[simulation.tick + 1] = new Commands();
            }

            elapsedTime -= 1 / ticksPerSecond;
            DoTick();
        }
    }

    bool HaveAllCommandsForNextTick() {
        return commandHistory.ContainsKey(simulation.tick + 1);
    }

    void RollForwardToTick(int tick) {
        Toolbox.Log($"RollForwardToTick {simulation.tick} -> {tick}");
        while (simulation.tick < tick) DoTick();
    }

    void DoTick() {
        Toolbox.Log($"DoTick {simulation.tick} -> {simulation.tick + 1}");
        safeGameState = simulation.DoTick(commandHistory[simulation.tick + 1]);
        safeTick = simulation.tick;
        Present(safeGameState);
    }

    void RollbackToTick(int tick) {
        Toolbox.Log($"RollbackToTick {simulation.tick} -> {tick}");
        safeGameState = simulation.RollbackToTick(tick);
        safeTick = simulation.tick;
        Present(safeGameState);
    }

    void Present(GameState gameState) {
        presenter?.Present(gameState);
    }
}
