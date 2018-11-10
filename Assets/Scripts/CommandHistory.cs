using System.Collections.Generic;
using System.Linq;

public class CommandHistory : Dictionary<int, PlayerCommands> {
    public void ChangeDirection(int tick, int playerId, DirectionEnum direction) {
        EnsureCommandsExistForPlayer(tick, playerId);
        this [tick][playerId].changeDirection = direction;
    }

    public void AddPlayer(int tick, int id) {
        EnsureCommandsExistForTick(tick);
        this [tick].serverCommands.newPlayerIds = this [tick].serverCommands.newPlayerIds.Concat(new int[] { id }).ToArray();
        this [tick][id] = new Commands();
    }

    public void ReceiveOtherPlayerCommand(PlayerCommandsMessage playerCommandsMessage) {
        EnsureCommandsExistForTick(playerCommandsMessage.tick);
        this [playerCommandsMessage.tick][playerCommandsMessage.playerId] = playerCommandsMessage.commands;
        this [playerCommandsMessage.tick][playerCommandsMessage.playerId].complete = true;
    }

    public void ReceiveServerCommand(ServerCommandsMessage serverCommandsMessage) {
        EnsureCommandsExistForTick(serverCommandsMessage.tick);
        this [serverCommandsMessage.tick].serverCommands.Merge(serverCommandsMessage.commands);
        this [serverCommandsMessage.tick].serverCommands.complete = true;
    }

    public void CompletePlayersCommands(int tick, int playerId) {
        EnsureCommandsExistForPlayer(tick, playerId);
        this [tick][playerId].complete = true;
    }

    public void CompleteServerCommandsAtTick(int tick) {
        EnsureCommandsExistForTick(tick);
        this [tick].serverCommands.complete = true;
    }

    public bool HavePlayerInputForTick(int tick, int playerId) {
        return this.ContainsKey(tick) && this [tick].ContainsKey(playerId) && this [tick][playerId].complete;
    }

    public bool HaveServerCommandsForTick(int tick) {
        return this.ContainsKey(tick) && this [tick].serverCommands.complete;
    }

    void EnsureCommandsExistForPlayer(int tick, int playerId) {
        EnsureCommandsExistForTick(tick);
        if (this [tick].ContainsKey(playerId) == false) {
            this [tick][playerId] = new Commands();
        }
    }

    void EnsureCommandsExistForTick(int tick) {
        if (this.ContainsKey(tick) == false) {
            this [tick] = new PlayerCommands();
        }
    }
}
