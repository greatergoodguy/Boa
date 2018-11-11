using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class Scheduler : MonoBehaviour {
    public static Scheduler I;

    public Presenter presenter;

    Simulation simulation;
    CommandHistory commandHistory = new CommandHistory();
    Clock clock;

    GameState safeGameState;
    int safeTick = 0;
    int playerStartTick = int.MaxValue;
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
        presenter.Clean();
        simulation = null;
        commandHistory.Clear();
        clock = null;
        safeGameState = new GameState();
        safeTick = 0;
        playerStartTick = int.MaxValue;
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
        }

        DoNormalTickStuff();
    }

    void CheckLocalPlayerInput() {
        if (safeGameState.players.Contains(Client.playerId) == false) return;

        var tickToUse = safeTick + 1;

        if (HaveLocalPlayerCommandsForNextTick()) tickToUse++;

        ClientCommander.CheckLocalPlayerInput(commandHistory.GetCommandsForPlayer(tickToUse, Client.playerId));
    }

    void DoNormalTickStuff() {
        if (clock.Tock(Time.deltaTime)) {
            if (HaveAllRequiredCommands() == false) return;
            DoTick();
        }
    }

    bool HaveAllRequiredCommands() {
        if (GameOffline.isOffline) return true;

        if (Client.isClient) CompleteLocalPlayerCommands();

        if (HaveAllOtherClientCommandsForNextTick() == false) {
            Debug.Log("Waiting for client commands...");
            return false;
        }

        if (Server.isServer) {
            DoServerCommandsDefault();
        } else if (HaveServerCommandsForNextTick() == false) {
            Debug.Log("Waiting for server commands...");
            return false;
        }

        return true;
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

    void DoTick() {
        Toolbox.Log($"DoTick {safeTick} -> {safeTick + 1}");
        safeGameState = simulation.DoTick(commandHistory.GetCommands(safeTick + 1));
        safeTick++;
        Present(safeGameState);
        clock.Reset();
    }

    void Present(GameState gameState) {
        presenter.Present(gameState);
        ServerPresenter.elapsedTime = clock.elapsedTime;
    }
}
