using System.Collections.Generic;
using System.Linq;

public class PlayerCommands : Dictionary<int, Commands> {
    public readonly ServerCommands serverCommands = new ServerCommands();

    public PlayerCommands(int[] players) {
        foreach (var playerId in players) {
            this [playerId] = new Commands();
        }
    }
}

public class Commands {
    public bool complete;
    public DirectionEnum changeDirection;
}

public class ServerCommands {
    public bool complete;
    public int[] newPlayerIds = new int[0];
    public int[] leftPlayerIds = new int[0];

    public void Merge(ServerCommands moreCommands) {
        complete = moreCommands.complete;
        newPlayerIds = new HashSet<int>(newPlayerIds).Union(moreCommands.newPlayerIds).ToArray();
        leftPlayerIds = new HashSet<int>(leftPlayerIds).Union(moreCommands.leftPlayerIds).ToArray();
    }
}
