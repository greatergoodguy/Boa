using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Presenter : MonoBehaviour, IPresenter<GameState> {
	void Start() {

	}

	void Update() {

	}

	public void Present(GameState gameState) {
		ServerPresenter.I.Present(gameState);
		
		if (Client.isClient) {
			AllSnakesPresenter.I.Present(gameState.snakes);
			AllApplesPresenter.I.Present(gameState.apples);
		}
	}
}
