using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientCommander : MonoBehaviour {
	void Start() {

	}

	void Update() {

	}

	public static void CheckLocalPlayerInput(Commands localPlayerCommands) {
		if (DG_Input.GoLeft()) {
			localPlayerCommands.ChangeDirection(DirectionEnum.Left);
		}
		if (DG_Input.GoUp()) {
			localPlayerCommands.ChangeDirection(DirectionEnum.Up);
		}
		if (DG_Input.GoRight()) {
			localPlayerCommands.ChangeDirection(DirectionEnum.Right);
		}
		if (DG_Input.GoDown()) {
			localPlayerCommands.ChangeDirection(DirectionEnum.Down);
		}
	}
}
