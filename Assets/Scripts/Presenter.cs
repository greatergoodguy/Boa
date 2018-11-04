using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Presenter : MonoBehaviour, IPresenter<GameState> {

	void Start() {

	}

	void Update() {

	}

	public void Present(GameState gameState) {
		AllSnakesPresenter.I.Present(gameState.snakes);
	}
}
