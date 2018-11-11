using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameOnlineMenu : GameStateMachine {
	public NetworkManagerHUD NetworkManagerHUD;

	public override void Enter() {
		NetworkManagerHUD.enabled = true;
	}

	public override void Exit() {
		NetworkManagerHUD.enabled = false;
	}

	public override GameStateMachine GetNextState() {
		if (NetworkManager.singleton.client != null && NetworkManager.singleton.client.isConnected) {
			return GetComponent<GameJoining>();
		} else {
			return null;
		}
	}
}
