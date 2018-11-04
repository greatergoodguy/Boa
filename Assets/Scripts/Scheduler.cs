using System.Collections.Generic;
using UnityEngine;

public class Scheduler : MonoBehaviour {

    public Presenter presenter;
    public float ticksPerSecond;

    Simulation simulation = new Simulation();
    CommandHistory commandHistory = new CommandHistory();

    bool manualTickDebugMode;
    float elapsedTime = 0;
    int safeTick = 0;

    void Awake() { }

    void Start() {
        presenter.Present(simulation.GetInitialGameState());
    }

    void Update() {
        CheckLocalPlayerInput();

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

    public void RollForwardToTick(int tick) {
        Toolbox.Log($"RollForwardToTick {simulation.tick} -> {tick}");
        while (simulation.tick < tick) DoTick();
    }

    void DoTick() {
        Toolbox.Log($"DoTick {simulation.tick} -> {simulation.tick + 1}");
        presenter.Present(simulation.DoTick(commandHistory[simulation.tick + 1]));
        safeTick = Mathf.Max(simulation.tick, safeTick);
    }

    void RollbackToTick(int tick) {
        Toolbox.Log($"RollbackToTick {simulation.tick} -> {tick}");
        presenter.Present(simulation.RollbackToTick(tick));
    }
}

class CommandHistory : Dictionary<int, Commands> { }

public struct Commands {
    public readonly DirectionEnum changeDirection;

    public Commands(DirectionEnum changeDirection) {
        this.changeDirection = changeDirection;
    }
}
