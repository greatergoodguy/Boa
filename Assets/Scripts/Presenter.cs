using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Presenter : MonoBehaviour, IPresenter<GameState> {
	public bool isServer;

	void Start() {

	}

	void Update() {

	}

	public void Present(GameState gameState) {
		ServerPresenter.I.Present(gameState);
		
		if (!isServer) {
			AllSnakesPresenter.I.Present(gameState.snakes);
		}
	}
}
