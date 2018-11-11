using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DG_NetworkManager : NetworkManager {
	void Start() {

	}

	void Update() {

	}
	
	public override void OnServerDisconnect(NetworkConnection conn) {
		if (conn.lastError != NetworkError.Ok) {
			Debug.Log("A client was disconnected due to error: " + conn.lastError);
		}

		Debug.Log("A client disconnected from the server: " + conn);

		Server.I.OnPlayerDisconnect(conn.connectionId);
	}
}
