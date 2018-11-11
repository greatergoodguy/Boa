using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameMenu : GameStateMachine {
	bool online;
	bool offline;

	public override GameStateMachine GetNextState() {
		if (online) {
			return GetComponent<GameOnlineMenu>();
		} else if (offline) {
			return GetComponent<GameOffline>();
		} else {
			return null;
		}
	}

	public override void Enter() {
		online = false;
		offline = false;
		GameUI.I.ToggleMainMenu(true);
	}

	public override void Exit() {
		GameUI.I.ToggleMainMenu(false);
	}

	public void OnOnlineButtonClick() {
		online = true;
	}

	public void OnOfflineButtonClick() {
		offline = true;
	}
}
