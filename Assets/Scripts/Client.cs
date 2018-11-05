using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Client : MonoBehaviour {
	public static Client I;

	void Awake() {
		I = this;
	}

	void Start() { }

	public void OnJoining() {
		NetworkManager.singleton.client.RegisterHandler(NetworkMessages.JSONMessage, (NetworkMessage msg) => {
			Debug.Log("GOT MESSAGE: " + msg.ReadMessage<JSONMessage>().json);
		});
		NetworkManager.singleton.client.Send(NetworkMessages.JSONMessage, new JSONMessage() { json = "ping" });
	}

	void Update() {

	}
}

public class JSONMessage : MessageBase {
	public string json;
}
