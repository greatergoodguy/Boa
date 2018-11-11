using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Presenter : MonoBehaviour, IPresenter<GameState> {
	public void Present(GameState gameState) {
		ServerPresenter.I.Present(gameState);

		if (Client.isClient) {
			AllSnakesPresenter.I.Present(gameState.snakes);
			AllApplesPresenter.I.Present(gameState.apples);
			WallsPresenter.I.Present(gameState.walls);
		}
	}

	public void Clean() {
		ServerPresenter.I.Clean();

		if (Client.isClient) {
			AllSnakesPresenter.I.Clean();
			AllApplesPresenter.I.Clean();
			WallsPresenter.I.Clean();
		}
	}
}
