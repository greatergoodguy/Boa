using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameMenu : GameStateMachine {

	public override GameStateMachine GetNextState() {
		return null;
	}

	public override void Enter() {
		GameUI.I.ToggleMainMenu(true);
	}

	public override void Exit() {
		GameUI.I.ToggleMainMenu(false);
	}
}
