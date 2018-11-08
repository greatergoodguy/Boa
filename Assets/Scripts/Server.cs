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
	public static bool isServer;
	public bool LAN;

	Scheduler scheduler;
	DG_NetworkManager networkManager;

	void Awake() {
		Debug.Log("Server Awake");
		isServer = true;
		scheduler = GetComponent<Scheduler>();
		networkManager = GetComponent<DG_NetworkManager>();
	}

	void Start() {
		Debug.Log("Server Start");

		RegisterHandlers();

		if (LAN) {
			StartLANServer();
		} else {
			CreateMatch();
		}
	}

	void RegisterHandlers() {
		RegisterHandler(JSONMessageType, (NetworkMessage msg) => {
			Debug.Log("GOT MESSAGE JSONMessageType: " + msg.ReadMessage<JSONMessage>().json);
			SendToAll(JSONMessageType, new JSONMessage() { json = "pong" });
		});
		RegisterHandler(RequestGameStateType, (NetworkMessage msg) => {
			Debug.Log("GOT MESSAGE RequestGameStateType from " + msg.conn.connectionId);
			var gameStateAndCommands = Scheduler.I.GetGameStateAndCommands();
			Debug.Log("SENDING: " + JsonConvert.SerializeObject(gameStateAndCommands));
			SendToClient(msg.conn.connectionId, RequestGameStateType, gameStateAndCommands);
		});
		RegisterHandler(SnakeSpawnType, (NetworkMessage msg) => {
			Debug.Log("GOT MESSAGE SnakeSpawnType from " + msg.conn.connectionId);

			// Debug.Log("SENDING: " + JsonConvert.SerializeObject(gameStateAndCommands));
			// SendToClient(msg.conn.connectionId, RequestGameStateType, gameStateAndCommands);
		});
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
		scheduler.GoServer();
	}

	void Update() {

	}
}
