using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class Client : MonoBehaviour {
	public static Client I;
	public static bool isClient;
	public static int playerId;

	public NetworkClient client { get; private set; }

	void Awake() {
		I = this;
		isClient = true;
	}

	void Start() { }

	public void OnJoining() {
		client = NetworkManager.singleton.client;
		playerId = client.connection.connectionId;

		RegisterHandlers();

		client.Send(DG_MsgType.JSONMessage, new JSONMessage() { json = "ping" });

		client.Send(DG_MsgType.PlayerJoin, new EmptyMessage());
	}

	void RegisterHandlers() {
		client.RegisterHandler(DG_MsgType.JSONMessage, (NetworkMessage msg) => {
			Debug.Log("GOT MESSAGE JSONMessage: " + msg.ReadMessage<JSONMessage>().json);
		});

		client.RegisterHandler(DG_MsgType.PlayerJoin, (NetworkMessage msg) => {
			var gameStateMessage = msg.ReadMessage<PlayerJoin>();
			Debug.Log("GOT MESSAGE PlayerJoin: " + JsonConvert.SerializeObject(gameStateMessage));
			Scheduler.I.LoadGameStateAndCommands(gameStateMessage);
		});

		client.RegisterHandler(DG_MsgType.ServerCommand, (NetworkMessage msg) => {
			var serverCommandsMessage = msg.ReadMessage<ServerCommandsMessage>();
			Debug.Log("GOT MESSAGE ServerCommand: " + JsonConvert.SerializeObject(serverCommandsMessage));
			Scheduler.I.OnServerCommand(serverCommandsMessage);
		});

		client.RegisterHandler(DG_MsgType.PlayerCommand, (NetworkMessage msg) => {
			var playerCommandsMessage = msg.ReadMessage<PlayerCommandsMessage>();
			Debug.Log("GOT MESSAGE PlayerCommand: " + JsonConvert.SerializeObject(playerCommandsMessage));
			if (playerCommandsMessage.playerId == playerId) return;
			Scheduler.I.OnPlayerCommand(playerCommandsMessage);
		});
	}

	public void SendClientCommand(int tick, Commands commands) {
		client.Send(DG_MsgType.PlayerCommand, new PlayerCommandsMessage() {
			tick = tick,
				commands = commands,
				playerId = playerId
		});
	}

	void Update() {

	}
}
