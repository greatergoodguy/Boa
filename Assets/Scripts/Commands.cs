using System.Collections.Generic;

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
    public bool spawn;
}

public class ServerCommands {
    public bool complete;
    public int[] newPlayerIds = new int[0];
    public int[] leftPlayerIds = new int[0];
}
