using System.Collections.Generic;
using System.Linq;

public class CommandHistory : Dictionary<int, PlayerCommands> {
    public void SpawnSnake(int tick, int ownerNetId) {
        this [tick][ownerNetId].spawn = true;
    }

    public void ChangeDirection(int tick, int ownerNetId, DirectionEnum direction) {
        this [tick][ownerNetId].changeDirection = direction;
    }

    public void AddPlayer(int tick, int id) {
        this [tick].serverCommands.newPlayerIds = this [tick].serverCommands.newPlayerIds.Concat(new int[] { id }).ToArray();
    }
}
