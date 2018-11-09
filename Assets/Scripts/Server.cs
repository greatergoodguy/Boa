using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using static UnityEngine.Networking.NetworkServer;

public class Server : MonoBehaviour {
	public static bool isServer;
	public static Server I;
	public bool LAN;

	Scheduler scheduler;
	DG_NetworkManager networkManager;

	void Awake() {
		Debug.Log("Server Awake");
		isServer = true;
		scheduler = GetComponent<Scheduler>();
		networkManager = GetComponent<DG_NetworkManager>();
		I = this;
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
		RegisterHandler(DG_MsgType.JSONMessage, (NetworkMessage msg) => {
			Debug.Log("GOT MESSAGE JSONMessage: " + msg.ReadMessage<JSONMessage>().json);
			SendToAll(DG_MsgType.JSONMessage, new JSONMessage() { json = "pong" });
		});
		RegisterHandler(DG_MsgType.PlayerJoin, (NetworkMessage msg) => {
			Debug.Log("GOT MESSAGE PlayerJoin from " + msg.conn.connectionId);
			var gameStateAndCommands = Scheduler.I.GetGameStateAndCommandsAndAddPlayer(msg.conn.connectionId);
			Debug.Log("SENDING: " + JsonConvert.SerializeObject(gameStateAndCommands));
			SendToClient(msg.conn.connectionId, DG_MsgType.PlayerJoin, gameStateAndCommands);
		});
		RegisterHandler(DG_MsgType.PlayerCommand, (NetworkMessage msg) => {
			Debug.Log("GOT MESSAGE PlayerCommand from " + msg.conn.connectionId);
			var playerCommandsMsg = msg.ReadMessage<PlayerCommandsMessage>();
			// Debug.Log("SENDING: " + JsonConvert.SerializeObject(gameStateAndCommands));
			// SendToClient(msg.conn.connectionId, RequestGameStateType, gameStateAndCommands);
			SendToAll(DG_MsgType.PlayerCommand, playerCommandsMsg);
			Scheduler.I.OnPlayerCommand(playerCommandsMsg);
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

	public void SendServerCommandToClients(int tick, ServerCommands serverCommands) {
		SendToAll(DG_MsgType.ServerCommand, new ServerCommandsMessage() {
			tick = tick,
				commands = serverCommands
		});
	}

	void Update() {

	}
}
