using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using static NetworkMessageTypes;

public class Client : MonoBehaviour {
	public static Client I;

	NetworkClient client;

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
			var json = msg.ReadMessage<RequestGameStateMessage>();
			Debug.Log("GOT MESSAGE RequestGameStateType: " + JsonConvert.SerializeObject(json));
			Scheduler.I.LoadGameStateAndCommands(json);
		});

		client.Send(JSONMessageType, new JSONMessage() { json = "ping" });

		client.Send(RequestGameStateType, new EmptyMessage());
	}

	void Update() {

	}
}
