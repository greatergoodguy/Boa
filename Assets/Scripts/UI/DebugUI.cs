using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugUI : MonoBehaviour {
	public Text otherDebugText;

	void Start() {

	}

	void Update() {
		var debugText = "";

		debugText += "stateMachineState: " + GameStateMachineManager.I?.state.GetType() + ServerStateMachineManager.I?.state.GetType() + "\n";
		debugText += "isPaused: " + Scheduler.I.clock?.paused + "\n";
		if (Client.playerId > 0) debugText += "clientPlayerId: " + Client.playerId + "\n";

		otherDebugText.text = debugText;
	}
}
