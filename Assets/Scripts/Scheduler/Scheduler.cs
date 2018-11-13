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
    public Clock clock { get; private set; }

    GameState safeGameState;
    int safeTick = 0;
    int playerStartTick = int.MaxValue;
    bool running = false;
    int localPlayerId;

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

    public int RemovePlayer(int playerId) {
        Debug.Log($"RemovePlayer safeTick: {safeTick} | playerId: {playerId}");

        commandHistory.RemovePlayer(safeTick + 1, playerId);

        return safeTick;
    }

    public void LoadGameStateAndCommands(PlayerJoin gameStateMessage) {
        Debug.Log("LoadGameStateAndCommands safeTick: " + safeTick);
        playerStartTick = gameStateMessage.playerStartTick;
        commandHistory = JsonConvert.DeserializeObject<CommandHistory>(gameStateMessage.commandHistoryJSON);
        Go(gameStateMessage.gameState, gameStateMessage.playerId, gameStateMessage.safeTick);
    }

    public void Go(GameState initialGameState, int localPlayerId, int tick = 0) {
        Debug.Log("Go initialGameState safeTick: " + safeTick);
        simulation = new Simulation(initialGameState, tick);
        safeGameState = initialGameState;
        safeTick = tick;
        this.localPlayerId = localPlayerId;
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
        Debug.Log($"OnPlayerCommand safeTick: {safeTick} player: {playerCommandsMessage.playerId} tick: {playerCommandsMessage.tick}");
        if (!running) return;
        commandHistory.ReceiveOtherPlayerCommand(playerCommandsMessage);
    }

    void Update() {
        if (!running) return;

        if (Client.isClient || GameOffline.isOffline) {
            CheckLocalPlayerInput();
        }

        DoNormalTickStuff();
    }

    void CheckLocalPlayerInput() {
        if (safeGameState.players.Contains(localPlayerId) == false) return;

        var tickToUse = safeTick + 1;

        if (HaveLocalPlayerCommandsForNextTick()) tickToUse++;

        ClientCommander.CheckLocalPlayerInput(commandHistory.GetCommandsForPlayer(tickToUse, localPlayerId));
    }

    void DoNormalTickStuff() {
        if (clock.Tock(Time.deltaTime)) {
            if (HaveAllRequiredCommands() == false) return;
            DoTick();
        }
    }

    bool HaveAllRequiredCommands() {
        if (GameOffline.isOffline) {
            commandHistory.CompletePlayersCommands(safeTick + 1, localPlayerId);
            return true;
        }

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

        commandHistory.CompletePlayersCommands(safeTick + 1, localPlayerId);
        Client.I.SendClientCommand(safeTick + 1, commandHistory.GetCommandsForPlayer(safeTick + 1, localPlayerId));
    }

    bool HaveLocalPlayerCommandsForNextTick() => commandHistory.HavePlayerInputForTick(safeTick + 1, localPlayerId);

    bool HaveLocalPlayerCommandsForNextNextTick() => commandHistory.HavePlayerInputForTick(safeTick + 2, localPlayerId);

    void DoServerCommandsDefault() {
        commandHistory.CompleteServerCommandsAtTick(safeTick + 1);
        Server.I.SendServerCommandToClients(safeTick + 1, commandHistory.GetServerCommands(safeTick + 1));
    }

    bool HaveAllOtherClientCommandsForNextTick() {
        Func<int, bool> NotLocalPlayer = (playerId) => playerId != localPlayerId;

        Func<int, bool> HavePlayerInput = (playerId) => commandHistory.HavePlayerInputForTick(safeTick + 1, playerId);

        return safeGameState.players.Where(NotLocalPlayer).All(HavePlayerInput);
    }

    bool HaveServerCommandsForNextTick() {
        return commandHistory.HaveServerCommandsForTick(safeTick + 1);
    }

    void DoTick() {
        Toolbox.Log($"DoTick {safeTick} -> {safeTick + 1}");
        safeGameState = simulation.DoTick(commandHistory.TakeCommands(safeTick + 1));
        safeTick++;
        Present(safeGameState);
        clock.Reset();
    }

    void Present(GameState gameState) {
        presenter.Present(gameState);
    }
}
