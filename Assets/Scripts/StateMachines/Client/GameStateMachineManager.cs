using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class GameStateMachineManager : MonoBehaviour {
	public static GameStateMachineManager I;

	GameStateMachine state;

	void Awake() {
		I = this;
	}

	void Start() {
		Toolbox.Log("GameStateMachineManager Start");
		state = GetComponent<GameMenu>();
		state.Enter();
	}

	void Update() {
		var nextState = state.GetNextState();

		if (nextState) {
			Toolbox.Log("GameStateMachineManager switching state: " + nextState.GetType().Name);
			state.Exit();
			state = nextState;
			state.Enter();
		}
	}
}
