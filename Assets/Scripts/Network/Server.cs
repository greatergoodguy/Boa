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
	public static int playerCount;

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
			Debug.Log("GOT MESSAGE JSONMessage from player " + msg.conn.connectionId + ": " + msg.ReadMessage<JSONMessage>().json);
			SendToAll(DG_MsgType.JSONMessage, new JSONMessage() { json = "pong" });
		});
		RegisterHandler(DG_MsgType.PlayerJoin, (NetworkMessage msg) => {
			Debug.Log("GOT MESSAGE PlayerJoin from player " + msg.conn.connectionId);
			var gameStateAndCommands = Scheduler.I.GetGameStateAndCommandsAndAddPlayer(msg.conn.connectionId);
			Debug.Log("SENDING: " + JsonConvert.SerializeObject(gameStateAndCommands));
			SendToClient(msg.conn.connectionId, DG_MsgType.PlayerJoin, gameStateAndCommands);
			playerCount++;
		});
		RegisterHandler(DG_MsgType.PlayerCommand, (NetworkMessage msg) => {
			var playerCommandsMsg = msg.ReadMessage<PlayerCommandsMessage>();
			Debug.Log("GOT MESSAGE PlayerCommand from player " + msg.conn.connectionId + ": " + JsonConvert.SerializeObject(playerCommandsMsg));
			SendAndSchedulePlayerCommand(playerCommandsMsg);
		});
	}

	public void OnPlayerDisconnect(int playerId) {
		var tick = Scheduler.I.RemovePlayer(playerId);

		// TODO Might cause desync, maybe only send if we don't have that players commands for this tick
		SendAndSchedulePlayerCommand(new PlayerCommandsMessage() {
			commands = new Commands() { complete = true },
				playerId = playerId,
				tick = tick + 1
		});
	}

	void SendAndSchedulePlayerCommand(PlayerCommandsMessage playerCommandsMsg) {
		SendToAll(DG_MsgType.PlayerCommand, playerCommandsMsg);
		Scheduler.I.OnPlayerCommand(playerCommandsMsg);
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
		scheduler.Go(InitialGameStates.ServerInitialGameState, -1);
	}

	public void SendServerCommandToClients(int tick, ServerCommands serverCommands) {
		var serverCommandsMessage = new ServerCommandsMessage() {
			tick = tick,
				commands = serverCommands
		};
		Debug.Log("SENDING MESSAGE ServerCommand: " + JsonConvert.SerializeObject(serverCommandsMessage));
		SendToAll(DG_MsgType.ServerCommand, serverCommandsMessage);
	}

	void Update() {

	}
}
