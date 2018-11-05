using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameMenu : GameStateMachine {

	void Update() { }

	public override GameStateMachine GetNextState() {
		if (NetworkManager.singleton.client != null && NetworkManager.singleton.client.isConnected) {
			return GetComponent<GameJoining>();
		} else {
			return null;
		}
	}

	public override void Enter() {
		GameUI.I.ToggleMainMenu(true);
	}

	public override void Exit() {
		GameUI.I.ToggleMainMenu(false);
	}
}
