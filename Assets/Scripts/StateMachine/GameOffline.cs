using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOffline : GameStateMachine {
	public static bool isOffline;

	public bool quitButton { get; set; }

	public override void Enter() {
		isOffline = true;
		quitButton = false;
		GameUI.I.ToggleInGameMenu(true);
		Scheduler.I.Go(InitialGameStates.OfflineInitialGameState);
	}

	public override void Exit() {
		isOffline = false;
		GameUI.I.ToggleInGameMenu(false);
		Scheduler.I.Stop();
	}

	public override GameStateMachine GetNextState() {
		if (quitButton) {
			return GetComponent<GameMenu>();
		} else {
			return null;
		}
	}
}
