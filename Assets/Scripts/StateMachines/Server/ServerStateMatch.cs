using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerStateMatch : GameStateMachine {
	public override void Enter() {
		Scheduler.I.Unpause();
	}

	public override void Exit() {
		Scheduler.I.Stop();
	}

	public override GameStateMachine GetNextState() {
		if (Server.playerCount == 0) {
			return GetComponent<ServerStateEmpty>();
		} else {
			return null;
		}
	}
}
