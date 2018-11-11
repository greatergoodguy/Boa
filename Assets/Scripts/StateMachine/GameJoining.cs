using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameJoining : GameStateMachine {
	public bool quitButton { get; set; }

	public override void Enter() {
		quitButton = false;
		GameUI.I.ToggleInGameMenu(true);
		Client.I.OnJoining();
	}

	public override void Exit() {
		GameUI.I.ToggleInGameMenu(false);
		// TODO Tell Client to send player leave message to server
		Scheduler.I.Stop();
		Client.I.Disconnect();
	}

	public override GameStateMachine GetNextState() {
		if (quitButton) {
			return GetComponent<GameMenu>();
		} else {
			return null;
		}
	}
}
