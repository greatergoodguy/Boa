using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class Scheduler : MonoBehaviour {
    public static Scheduler I;

    public Presenter presenter;
    public Commander commander;

    Simulation simulation;
    CommandHistory commandHistory = new CommandHistory();
    Clock clock;

    // bool manualTickDebugMode;
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

        commandHistory.AddPlayer(safeTick + 1, playerId);
        playerStartTick = safeTick + 2;

        return new PlayerJoin() {
            commandHistoryJSON = JsonConvert.SerializeObject(commandHistory),
                gameState = safeGameState,
                safeTick = safeTick,
                playerStartTick = playerStartTick,
                playerId = playerId,
        };
    }

    public void LoadGameStateAndCommands(PlayerJoin gameStateMessage) {
        Debug.Log("LoadGameStateAndCommands safeTick: " + safeTick);
        playerStartTick = gameStateMessage.playerStartTick;
        commandHistory = JsonConvert.DeserializeObject<CommandHistory>(gameStateMessage.commandHistoryJSON);
        Go(gameStateMessage.gameState, gameStateMessage.safeTick);
    }

    public void Go(GameState initialGameState, int tick = 0) {
        Debug.Log("Go initialGameState safeTick: " + safeTick);
        simulation = new Simulation(initialGameState, tick);
        safeGameState = initialGameState;
        safeTick = tick;
        Go();
    }

    void Go() {
        Debug.Log("Go safeTick: " + safeTick);
        clock = new Clock();
        running = true;
        Present(safeGameState);
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

        var tickToUse = safeTick + 1;

        if (HaveLocalPlayerCommandsForNextTick()) tickToUse++;

        ClientCommander.CheckLocalPlayerInput(commandHistory.GetCommandsForPlayer(tickToUse, Client.playerId));
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
        if (clock.Tock(Time.deltaTime)) {
            if (Client.isClient) CompleteLocalPlayerCommands();

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

            DoTick();
        }
    }

    void CompleteLocalPlayerCommands() {
        if (safeTick + 1 < playerStartTick) return;
        if (HaveLocalPlayerCommandsForNextTick()) return;

        commandHistory.CompletePlayersCommands(safeTick + 1, Client.playerId);
        Client.I.SendClientCommand(safeTick + 1, commandHistory.GetCommandsForPlayer(safeTick + 1, Client.playerId));
    }

    bool HaveLocalPlayerCommandsForNextTick() => commandHistory.HavePlayerInputForTick(safeTick + 1, Client.playerId);

    bool HaveLocalPlayerCommandsForNextNextTick() => commandHistory.HavePlayerInputForTick(safeTick + 2, Client.playerId);

    void DoServerCommandsDefault() {
        commandHistory.CompleteServerCommandsAtTick(safeTick + 1);
        Server.I.SendServerCommandToClients(safeTick + 1, commandHistory.GetServerCommands(safeTick + 1));
    }

    bool HaveAllOtherClientCommandsForNextTick() {
        Func<int, bool> NotLocalPlayer = (playerId) => playerId != Client.playerId;

        Func<int, bool> HavePlayerInput = (playerId) => commandHistory.HavePlayerInputForTick(safeTick + 1, playerId);

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
        safeGameState = simulation.DoTick(commandHistory.GetCommands(safeTick + 1));
        safeTick++;
        Present(safeGameState);
        clock.Reset();
    }

    // void RollbackToTick(int tick) {
    //     Toolbox.Log($"RollbackToTick {safeTick} -> {tick}");
    //     safeGameState = simulation.RollbackToTick(tick);
    //     safeTick = tick;
    //     Present(safeGameState);
    // }

    void Present(GameState gameState) {
        presenter.Present(gameState);
        ServerPresenter.elapsedTime = clock.elapsedTime;
    }
}
