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
                playerStartTick = playerStartTick,
                playerId = playerId,
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
        // commandHistory = gameStateMessage.commandHistory;
        // commandHistory[safeTick + 1] = new PlayerCommands(safeGameState.players);
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
        // commandHistory[safeTick + 1] = new PlayerCommands(safeGameState.players);
        Present(safeGameState);
        running = true;
    }

    public void Stop() {
        running = false;
    }

    public void OnServerCommand(ServerCommandsMessage serverCommandsMessage) {
        if (!running) return;
        commandHistory.ReceiveServerCommand(serverCommandsMessage);
    }

    public void OnPlayerCommand(PlayerCommandsMessage playerCommandsMessage) {
        if (!running) return;
        commandHistory.ReceiveOtherPlayerCommand(playerCommandsMessage);
    }

    void Update() {
        if (!running) return;

        if (Client.isClient) {
            CheckLocalPlayerInput();

            // if (DG_Input.ToggleManualTick()) {
            //     manualTickDebugMode = !manualTickDebugMode;
            // }
        }

        // if (manualTickDebugMode) {
        //     DoManualTickStuff();
        // } else {
            DoNormalTickStuff();
        // }
    }

    // TODO Move to new script that sets a variable whenever a player presses a key
    // then the Scheduler reaches out to read the input once per tick and clears it when it does
    void CheckLocalPlayerInput() {
        if (safeGameState.players.Contains(Client.playerId) == false) return;
        if (HaveLocalPlayerCommandsForNextTick()) return;
        if (DG_Input.GoLeft()) {
            commandHistory.ChangeDirection(safeTick + 1, Client.playerId, DirectionEnum.Left);
        }
        if (DG_Input.GoUp()) {
            commandHistory.ChangeDirection(safeTick + 1, Client.playerId, DirectionEnum.Up);
        }
        if (DG_Input.GoRight()) {
            commandHistory.ChangeDirection(safeTick + 1, Client.playerId, DirectionEnum.Right);
        }
        if (DG_Input.GoDown()) {
            commandHistory.ChangeDirection(safeTick + 1, Client.playerId, DirectionEnum.Down);
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
            if (Client.isClient) CompleteLocalPlayerCommands();

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

    void CompleteLocalPlayerCommands() {
        if (safeTick + 1 < playerStartTick) return;
        if (HaveLocalPlayerCommandsForNextTick()) return;

        commandHistory.CompletePlayersCommands(safeTick + 1, Client.playerId);
        Client.I.SendClientCommand(safeTick + 1, commandHistory[safeTick + 1][Client.playerId]);
    }

    bool HaveLocalPlayerCommandsForNextTick() {
        return commandHistory.HavePlayerInputForTick(safeTick + 1, Client.playerId);
    }

    void DoServerCommandsDefault() {
        commandHistory.CompleteServerCommandsAtTick(safeTick + 1);
        Server.I.SendServerCommandToClients(safeTick + 1, commandHistory[safeTick + 1].serverCommands);
    }

    bool HaveAllOtherClientCommandsForNextTick() {
        Func<int, bool> NotLocalPlayer = (playerId) => playerId != Client.playerId;

        Func<int, bool> HavePlayerInput = (playerId) => commandHistory.HavePlayerInputForTick(safeTick + 1, playerId);

        // Debug.Log("HaveAllCommandsForNextTick safeGameState.players: " + safeGameState.players);
        return safeGameState.players.Where(NotLocalPlayer).All(HavePlayerInput);
    }

    bool HaveServerCommandsForNextTick() {
        return commandHistory.HaveServerCommandsForTick(safeTick + 1);
    }

    // void RollForwardToTick(int tick) {
    //     Toolbox.Log($"RollForwardToTick {simulation.tick} -> {tick}");
    //     while (simulation.tick < tick) DoTick();
    // }

    void DoTick() {
        Toolbox.Log($"DoTick {safeTick} -> {safeTick + 1}");
        safeGameState = simulation.DoTick(commandHistory[safeTick + 1]);
        safeTick++;
        // commandHistory[safeTick + 1] = new PlayerCommands(safeGameState.players);
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
        ServerPresenter.elapsedTime = elapsedTime;
    }
}
