﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class Server : MonoBehaviour {
	Scheduler scheduler;
	DG_NetworkManager networkManager;

	void Awake() {
		Debug.Log("Server Awake");
		scheduler = GetComponent<Scheduler>();
		networkManager = GetComponent<DG_NetworkManager>();
	}

	void Start() {
		Debug.Log("Server Start");
		NetworkServer.RegisterHandler(NetworkMessages.JSONMessage, (NetworkMessage msg) => {
			Debug.Log("GOT MESSAGE: " + msg.ReadMessage<JSONMessage>().json);
			NetworkServer.SendToAll(NetworkMessages.JSONMessage, new JSONMessage() { json = "pong" });
		});

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
		// For creating LAN server
		// networkManager.StartServer();
	}

	public void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo) {
		networkManager.OnMatchCreate(success, extendedInfo, matchInfo);
		scheduler.Go();
	}

	void Update() {

	}
}
