﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerStateEmpty : GameStateMachine {
	public override void Enter() {
		Scheduler.I.Go(InitialGameStates.ServerInitialGameState, -1);
		Scheduler.I.Pause();
	}

	public override void Exit() {

	}

	public override GameStateMachine GetNextState() {
		if (Server.playerCount > 1) {
			return GetComponent<ServerStateMatch>();
		} else {
			return null;
		}
	}
}
