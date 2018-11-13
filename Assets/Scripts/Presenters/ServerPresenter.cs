﻿using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class ServerPresenter : MonoBehaviour, IPresenter<GameState> {
	public static ServerPresenter I;

	public Text serverDebugUIText;
	public bool verbose;

	string initialText;

	void Awake() {
		I = this;
		initialText = serverDebugUIText.text;
	}

	void Update() {

	}

	public void Present(GameState gameState) {
		var debugText = "";
		debugText += "tick: " + gameState.tick + "\n";
		debugText += "elapsedTime: " + Scheduler.I.clock?.elapsedTime + "\n";
		debugText += "players: " + JsonConvert.SerializeObject(gameState.players) + "\n";
		debugText += "snakes: " + GetHashString(JsonConvert.SerializeObject(gameState.snakes)) + (verbose ? " " + JsonConvert.SerializeObject(gameState.snakes) : "") + "\n";
		debugText += "apples: " + GetHashString(JsonConvert.SerializeObject(gameState.apples));
		serverDebugUIText.text = debugText;
	}

	static byte[] GetHash(string inputString) {
		HashAlgorithm algorithm = MD5.Create(); //or use SHA256.Create();
		return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
	}

	static string GetHashString(string inputString) {
		var stringBuilder = new StringBuilder();

		foreach (byte b in GetHash(inputString)) {
			stringBuilder.Append(b.ToString("X2"));
		}

		return stringBuilder.ToString();
	}

	public void Clean() {
		serverDebugUIText.text = initialText;
	}
}
