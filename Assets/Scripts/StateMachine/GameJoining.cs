using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameJoining : GameStateMachine {

	public override GameStateMachine GetNextState() {
		return null;
	}

	public override void Enter() {
		Client.I.OnJoining();
		// Scheduler.I.SyncWithServer();
	}
}
