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

    void Update() {
        CheckLocalPlayerInput();

        if (Input.GetKeyDown(KeyCode.P)) {
            manualTickDebugMode = !manualTickDebugMode;
        }

        if (manualTickDebugMode) {
            DoManualTickStuff();
        } else {
            elapsedTime += Time.deltaTime;

            if (elapsedTime > 1 / ticksPerSecond) {
                if (HaveAllCommandsForNextTick() == false) {
                    commandHistory[simulation.tick + 1] = new Commands();
                }

                elapsedTime -= 1 / ticksPerSecond;
                DoTick();
            }
        }
    }

    void CheckLocalPlayerInput() {
        if (Input.GetKeyDown(KeyCode.A)) {
            commandHistory[simulation.tick + 1] = new Commands(DirectionEnum.Left);
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            commandHistory[simulation.tick + 1] = new Commands(DirectionEnum.Up);
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            commandHistory[simulation.tick + 1] = new Commands(DirectionEnum.Right);
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            commandHistory[simulation.tick + 1] = new Commands(DirectionEnum.Down);
        }
    }

    bool HaveAllCommandsForNextTick() {
        return commandHistory.ContainsKey(simulation.tick + 1);
    }

    void DoManualTickStuff() {
        if (simulation.tick != safeTick && Input.GetKeyDown(KeyCode.N)) {
            if (Input.GetKey(KeyCode.LeftShift)) {
                RollForwardToTick(Mathf.Min(simulation.tick + 10, safeTick));
            } else {
                DoTick();
            }
        } else if (Input.GetKeyDown(KeyCode.B) && simulation.tick > 0) {
            if (Input.GetKey(KeyCode.LeftShift)) {
                RollbackToTick(Mathf.Max(simulation.tick - 10, 0));
            } else {
                RollbackToTick(simulation.tick - 1);
            }
        }
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
