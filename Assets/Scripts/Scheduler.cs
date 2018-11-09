using System;
using System.Collections.Generic;
using System.Linq;
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
    int playerStartTick = int.MaxValue;
    int newLocalCommandsTick;
    GameState safeGameState;
    bool running = false;

    void Awake() {
        Debug.Log("Awake safeTick: " + safeTick);
        I = this;
    }

    public PlayerJoin GetGameStateAndCommandsAndAddPlayer(int playerId) {
        Debug.Log("GetGameStateAndCommandsAndAddPlayer safeTick: " + safeTick);
        var playerStartTick = AddPlayer(playerId);

        return new PlayerJoin() {
            commandHistory = commandHistory,
                gameState = safeGameState,
                safeTick = safeTick,
                playerStartTick = playerStartTick
        };
    }

    /// <returns>tick where player can start sending commands</returns>
    int AddPlayer(int playerId) {
        commandHistory.AddPlayer(safeTick + 1, playerId);
        return safeTick + 2;
    }

    public void LoadGameStateAndCommands(PlayerJoin gameStateMessage) {
        Debug.Log("LoadGameStateAndCommands safeTick: " + safeTick);
        safeGameState = gameStateMessage.gameState;
        safeTick = gameStateMessage.safeTick;
        playerStartTick = gameStateMessage.playerStartTick;
        newLocalCommandsTick = safeTick + 1;
        // commandHistory = gameStateMessage.commandHistory;
        commandHistory[safeTick + 1] = new PlayerCommands(safeGameState.players);
        simulation.LoadGameState(safeTick, safeGameState);
        Go();
    }

    public void GoServer() {
        Debug.Log("GoServer safeTick: " + safeTick);
        safeGameState = simulation.GetInitialGameState();
        safeTick = 0;
        Go();
    }

    public void Go() {
        Debug.Log("Go safeTick: " + safeTick);
        commandHistory[safeTick + 1] = new PlayerCommands(safeGameState.players);
        Present(safeGameState);
        running = true;
    }

    public void Stop() {
        running = false;
    }

    public void OnServerCommand(ServerCommandsMessage serverCommandsMessage) {
        if (!running) return;
        commandHistory[serverCommandsMessage.tick].serverCommands.Merge(serverCommandsMessage.commands);
        commandHistory[serverCommandsMessage.tick].serverCommands.complete = true;
    }

    public void OnPlayerCommand(PlayerCommandsMessage playerCommandsMessage) {
        if (!running) return;
        commandHistory[playerCommandsMessage.tick][playerCommandsMessage.playerId] = playerCommandsMessage.commands;
        commandHistory[playerCommandsMessage.tick][playerCommandsMessage.playerId].complete = true;
    }

    void Update() {
        if (!running) return;

        if (NetworkManager.singleton.client != null) {
            CheckLocalPlayerInput();

            if (DG_Input.ToggleManualTick()) {
                manualTickDebugMode = !manualTickDebugMode;
            }
        }

        if (manualTickDebugMode) {
            DoManualTickStuff();
        } else {
            DoNormalTickStuff();
        }
    }

    void CheckLocalPlayerInput() {
        if (DG_Input.GoLeft()) {
            commandHistory.ChangeDirection(newLocalCommandsTick, Client.I.client.connection.connectionId, DirectionEnum.Left);
        }
        if (DG_Input.GoUp()) {
            commandHistory.ChangeDirection(newLocalCommandsTick, Client.I.client.connection.connectionId, DirectionEnum.Up);
        }
        if (DG_Input.GoRight()) {
            commandHistory.ChangeDirection(newLocalCommandsTick, Client.I.client.connection.connectionId, DirectionEnum.Right);
        }
        if (DG_Input.GoDown()) {
            commandHistory.ChangeDirection(newLocalCommandsTick, Client.I.client.connection.connectionId, DirectionEnum.Down);
        }
    }

    void DoManualTickStuff() {
        // if (simulation.tick != safeTick && DG_Input.NextTick()) {
        //     if (DG_Input.NextTickBig()) {
        //         RollForwardToTick(Mathf.Min(simulation.tick + 10, safeTick));
        //     } else {
        //         DoTick();
        //     }
        // } else if (DG_Input.BackTick() && simulation.tick > 0) {
        //     // if (DG_Input.BackTickBig()) {
        //     //     RollbackToTick(Mathf.Max(simulation.tick - 10, 0));
        //     // } else {
        //     //     RollbackToTick(simulation.tick - 1);
        //     // }
        // }
    }

    void DoNormalTickStuff() {
        elapsedTime += Time.deltaTime;

        if (elapsedTime > 1 / ticksPerSecond) {
            if (Client.isClient) DoLocalPlayerDefaultCommands();

            // Debug.Log("DoNormalTickStuff safeGameState.players: " + safeGameState.players);
            if (HaveAllOtherClientCommandsForNextTick() == false) {
                Debug.Log("Waiting for client commands...");
                return;
            }

            if (Server.isServer) {
                DoServerCommandsDefault();
            } else if (HaveServerCommandsForNextTick() == false) {
                Debug.Log("Waiting for server commands...");
                return;
            }

            elapsedTime -= 1 / ticksPerSecond;
            DoTick();
        }
    }

    void DoLocalPlayerDefaultCommands() {
        if (safeTick + 1 < playerStartTick) return;

        commandHistory[safeTick + 1][Client.playerId].complete = true;
        Client.I.SendClientCommand(safeTick + 1, commandHistory[safeTick + 1][Client.playerId]);
        newLocalCommandsTick = safeTick + 2;
    }

    void DoServerCommandsDefault() {
        commandHistory[safeTick + 1].serverCommands.complete = true;
        Server.I.SendServerCommandToClients(safeTick + 1, commandHistory[safeTick + 1].serverCommands);
    }

    bool HaveAllOtherClientCommandsForNextTick() {
        Func<int, bool> NotLocalPlayer = (playerId) => playerId != Client.playerId;

        Func<int, bool> HavePlayerInput = (playerId) => commandHistory[safeTick + 1][playerId].complete;

        // Debug.Log("HaveAllCommandsForNextTick safeGameState.players: " + safeGameState.players);
        return safeGameState.players.Where(NotLocalPlayer).All(HavePlayerInput);
    }

    bool HaveServerCommandsForNextTick() {
        return commandHistory[safeTick + 1].serverCommands.complete;
    }

    // void RollForwardToTick(int tick) {
    //     Toolbox.Log($"RollForwardToTick {simulation.tick} -> {tick}");
    //     while (simulation.tick < tick) DoTick();
    // }

    void DoTick() {
        Toolbox.Log($"DoTick {safeTick} -> {safeTick + 1}");
        safeGameState = simulation.DoTick(commandHistory[safeTick + 1]);
        safeTick++;
        commandHistory[safeTick + 1] = new PlayerCommands(safeGameState.players);
        Present(safeGameState);
    }

    // void RollbackToTick(int tick) {
    //     Toolbox.Log($"RollbackToTick {safeTick} -> {tick}");
    //     safeGameState = simulation.RollbackToTick(tick);
    //     safeTick = tick;
    //     Present(safeGameState);
    // }

    void Present(GameState gameState) {
        presenter?.Present(gameState);
    }
}
