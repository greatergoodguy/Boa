using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class ServerPresenter : MonoBehaviour, IPresenter<GameState> {
	public static ServerPresenter I;

	public Text serverDebugUIText;

	void Awake() {
		I = this;
	}

	void Update() {

	}

	public void Present(GameState gameState) {
		var debugText = "";
		debugText += "tick: " + JsonConvert.SerializeObject(gameState.tick) + "\n";
		debugText += "players: " + JsonConvert.SerializeObject(gameState.players) + "\n";
		debugText += "snakes: " + JsonConvert.SerializeObject(gameState.snakes) + "\n";
		debugText += "apples: " + JsonConvert.SerializeObject(gameState.apples) + "\n";
		serverDebugUIText.text = debugText;
	}
}
