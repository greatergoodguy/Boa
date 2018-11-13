using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameOnlineMenu : GameStateMachine {
	public NetworkManagerHUD NetworkManagerHUD;
	public bool quitButton { get; set; }

	public override void Enter() {
		NetworkManagerHUD.enabled = true;
		quitButton = false;
		GameUI.I.ToggleInGameMenu(true);
	}

	public override void Exit() {
		NetworkManagerHUD.enabled = false;
		GameUI.I.ToggleInGameMenu(false);
	}

	public override GameStateMachine GetNextState() {
		if (NetworkManager.singleton.client != null && NetworkManager.singleton.client.isConnected) {
			return GetComponent<GameJoining>();
		} else if (quitButton) {
			return GetComponent<GameMenu>();
		} else {
			return null;
		}
	}
}
