using System;
using System.Collections.Generic;
using System.Linq;

public class PlayerCommands {
    public ServerCommands serverCommands = new ServerCommands();
    public Dictionary<int, Commands> playerCommands = new Dictionary<int, Commands>();

    // public PlayerCommands(int[] players) {
    //     foreach (var playerId in players) {
    //         this [playerId] = new Commands();
    //     }
    // }
}

public class Commands {
    public bool complete;
    public DirectionEnum changeDirection;

    public void ChangeDirection(DirectionEnum newDirection) {
        if (complete) throw new InvalidOperationException("cannot change Commands if complete");
        this.changeDirection = newDirection;
    }

    public void SetComplete() {
        if (complete) throw new InvalidOperationException("already complete");
        complete = true;
    }
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
