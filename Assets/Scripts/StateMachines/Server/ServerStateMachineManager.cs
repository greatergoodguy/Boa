using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerStateMachineManager : MonoBehaviour {
	public static ServerStateMachineManager I;

	public GameStateMachine state { get; private set; }

	void Awake() {
		I = this;
	}

	void Start() {
		Toolbox.Log("ServerStateMachineManager Start");
		state = GetComponent<ServerStateEmpty>();
		state.Enter();
	}

	void Update() {
		var nextState = state.GetNextState();

		if (nextState) {
			Toolbox.Log("ServerStateMachineManager switching state: " + nextState.GetType().Name);
			state.Exit();
			state = nextState;
			state.Enter();
		}
	}
}
