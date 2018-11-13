using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerStateEmpty : GameStateMachine {
	public override void Enter() {

	}

	public override void Exit() {

	}
	
	public override GameStateMachine GetNextState() {
		if (Server.playerCount > 0) {
			return GetComponent<ServerStateMatch>();
		} else {
			return null;
		}
	}
}
