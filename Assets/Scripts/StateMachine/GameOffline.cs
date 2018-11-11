using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOffline : GameStateMachine {
	public override void Enter() {
		Scheduler.I.Go(InitialGameStates.OfflineInitialGameState);
	}

	public override GameStateMachine GetNextState() {
		return null;
	}
}
