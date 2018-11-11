using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOffline : GameStateMachine {
	public static bool isOffline;

	public override void Enter() {
		isOffline = true;
		Scheduler.I.Go(InitialGameStates.OfflineInitialGameState);
	}

	public override void Exit() {
		isOffline = false;
		Scheduler.I.Stop();
	}

	public override GameStateMachine GetNextState() {
		return null;
	}
}
