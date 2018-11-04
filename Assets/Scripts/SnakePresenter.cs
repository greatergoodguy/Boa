using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SnakePresenter : MonoBehaviour, IPresenter<SnakeState> {
	void Start() {

	}

	void Update() {

	}

	public void Present(SnakeState snakeState) {
		transform.position = snakeState.position.ToUnityVector2();
	}
}
