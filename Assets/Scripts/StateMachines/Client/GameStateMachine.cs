using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class GameStateMachine : MonoBehaviour {
	public abstract GameStateMachine GetNextState();

	public virtual void Enter() { }

	public virtual void Exit() { }
}
