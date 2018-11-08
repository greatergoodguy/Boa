using UnityEngine.Networking;

public static class NetworkMessageTypes {
    public const short JSONMessageType = 1000;
    public const short RequestGameStateType = 1001;
    public const short SnakeSpawnType = 1002;
}

public class EmptyMessage : MessageBase { }

public class JSONMessage : MessageBase {
    public string json;
}

public class RequestGameStateMessage : MessageBase {
    public GameState gameState;
    public int safeTick;
    public CommandHistory commandHistory;
}
