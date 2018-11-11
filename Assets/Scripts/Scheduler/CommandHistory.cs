using System.Collections.Generic;
using System.Linq;

public class CommandHistory {
    Dictionary<int, PlayerCommands> commands = new Dictionary<int, PlayerCommands>();
    
    public Commands GetCommandsForPlayer(int tick, int playerId) {
        EnsureCommandsExistForPlayer(tick, playerId);
        return commands[tick].playerCommands[playerId];
    }

    public ServerCommands GetServerCommands(int tick) {
        EnsureCommandsExistForTick(tick);
        return commands[tick].serverCommands;
    }

    public PlayerCommands GetCommands(int tick) {
        EnsureCommandsExistForTick(tick);
        return commands[tick];
    }

    public void AddPlayer(int tick, int id) {
        EnsureCommandsExistForTick(tick);
        commands[tick].serverCommands.newPlayerIds = commands[tick].serverCommands.newPlayerIds.Concat(new int[] { id }).ToArray();
        commands[tick].playerCommands[id] = new Commands();
    }

    public void ReceiveOtherPlayerCommand(PlayerCommandsMessage playerCommandsMessage) {
        EnsureCommandsExistForTick(playerCommandsMessage.tick);
        commands[playerCommandsMessage.tick].playerCommands[playerCommandsMessage.playerId] = playerCommandsMessage.commands;
    }

    public void ReceiveServerCommand(ServerCommandsMessage serverCommandsMessage) {
        EnsureCommandsExistForTick(serverCommandsMessage.tick);
        commands[serverCommandsMessage.tick].serverCommands.Merge(serverCommandsMessage.commands);
    }

    public void CompletePlayersCommands(int tick, int playerId) {
        EnsureCommandsExistForPlayer(tick, playerId);
        commands[tick].playerCommands[playerId].SetComplete();
    }

    public void CompleteServerCommandsAtTick(int tick) {
        EnsureCommandsExistForTick(tick);
        commands[tick].serverCommands.complete = true;
    }

    public bool HavePlayerInputForTick(int tick, int playerId) {
        return commands.ContainsKey(tick) && commands[tick].playerCommands.ContainsKey(playerId) && commands[tick].playerCommands[playerId].complete;
    }

    public bool HaveServerCommandsForTick(int tick) {
        return commands.ContainsKey(tick) && commands[tick].serverCommands.complete;
    }

    void EnsureCommandsExistForPlayer(int tick, int playerId) {
        EnsureCommandsExistForTick(tick);
        if (commands[tick].playerCommands.ContainsKey(playerId) == false) {
            commands[tick].playerCommands[playerId] = new Commands();
        }
    }

    void EnsureCommandsExistForTick(int tick) {
        if (commands.ContainsKey(tick) == false) {
            commands[tick] = new PlayerCommands();
        }
    }
}
