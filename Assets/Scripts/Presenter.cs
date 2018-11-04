using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Presenter : MonoBehaviour {
	public GameObject snakeHead;

	// Use this for initialization
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}

	public void Present(GameState gameState) {
		snakeHead.transform.position = gameState.snake.position.ToUnityVector2();
	}
}
