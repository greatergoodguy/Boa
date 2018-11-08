using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using static NetworkMessageTypes;

public class Client : MonoBehaviour {
	public static Client I;

	public NetworkClient client { get; private set; }

	void Awake() {
		I = this;
	}

	void Start() { }

	public void OnJoining() {
		client = NetworkManager.singleton.client;
		client.RegisterHandler(JSONMessageType, (NetworkMessage msg) => {
			Debug.Log("GOT MESSAGE JSONMessageType: " + msg.ReadMessage<JSONMessage>().json);
		});

		client.RegisterHandler(RequestGameStateType, (NetworkMessage msg) => {
			var gameStateMessage = msg.ReadMessage<RequestGameStateMessage>();
			Debug.Log("GOT MESSAGE RequestGameStateType: " + JsonConvert.SerializeObject(gameStateMessage));
			Scheduler.I.LoadGameStateAndCommands(gameStateMessage);
			RequestSnakeSpawnFromServer();
		});

		client.Send(JSONMessageType, new JSONMessage() { json = "ping" });

		client.Send(RequestGameStateType, new EmptyMessage());
	}

	void RequestSnakeSpawnFromServer() {
		client.Send(SnakeSpawnType, new EmptyMessage());
	}

	void Update() {

	}
}
