using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameJoining : GameStateMachine {
	public override void Enter() {
		Client.I.OnJoining();
	}

	public override GameStateMachine GetNextState() {
		return null;
	}
}
