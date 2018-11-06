using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using static NetworkMessageTypes;
using static UnityEngine.Networking.NetworkServer;

public class Server : MonoBehaviour {
	public bool LAN;

	Scheduler scheduler;
	DG_NetworkManager networkManager;

	void Awake() {
		Debug.Log("Server Awake");
		scheduler = GetComponent<Scheduler>();
		networkManager = GetComponent<DG_NetworkManager>();
	}

	void Start() {
		Debug.Log("Server Start");
		RegisterHandler(JSONMessageType, (NetworkMessage msg) => {
			Debug.Log("GOT MESSAGE JSONMessageType: " + msg.ReadMessage<JSONMessage>().json);
			SendToAll(JSONMessageType, new JSONMessage() { json = "pong" });
		});
		RegisterHandler(RequestGameStateType, (NetworkMessage msg) => {
			Debug.Log("GOT MESSAGE RequestGameStateType: " + RequestGameStateType);
			var gameStateAndCommands = Scheduler.I.SerializeGameStateAndCommands();
			Debug.Log("SENDING: " + JsonConvert.SerializeObject(gameStateAndCommands));
			SendToClient(msg.conn.connectionId, RequestGameStateType, gameStateAndCommands);
		});

		if (LAN) {
			StartLANServer();
		} else {
			CreateMatch();
		}
	}

	void CreateMatch() {
		networkManager.StartMatchMaker();
		networkManager.matchMaker.CreateMatch(
			matchName: DateTime.Now.ToString(),
			matchSize: 20,
			matchAdvertise: true,
			matchPassword: "",
			privateClientAddress: "",
			publicClientAddress: "",
			eloScoreForMatch : 0,
			requestDomain : 0,
			callback : OnMatchCreate
		);
	}

	void StartLANServer() {
		networkManager.StartServer();
		OnServerStart();
	}

	public void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo) {
		networkManager.OnMatchCreate(success, extendedInfo, matchInfo);
		OnServerStart();
	}

	void OnServerStart() {
		scheduler.GoWithDefaultGameState();
	}

	void Update() {

	}
}
