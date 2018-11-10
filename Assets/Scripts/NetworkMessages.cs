using UnityEngine.Networking;

public static class DG_MsgType {
    public const short JSONMessage = 1000;
    public const short PlayerJoin = 1001;
    public const short PlayerCommand = 1003;
    public const short ServerCommand = 1004;
}

public class EmptyMessage : MessageBase { }

public class JSONMessage : MessageBase {
    public string json;
}

public class PlayerJoin : MessageBase {
    public int safeTick;
    public int playerStartTick;
    public int playerId;
    public GameState gameState;
    public CommandHistory commandHistory;
}

public class PlayerCommandsMessage : MessageBase {
    public int playerId;
    public int tick;
    public Commands commands;
}

public class ServerCommandsMessage : MessageBase {
    public int tick;
    public ServerCommands commands;
}
