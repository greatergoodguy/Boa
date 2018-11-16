using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Presenter : MonoBehaviour, IPresenter<GameState> {
	public static int playerCount;
	
	public void Present(GameState gameState) {
		ServerPresenter.I.Present(gameState);
		playerCount = gameState.players.Length;

		if (Client.isClient) {
			AllSnakesPresenter.I.Present(gameState.snakes);
			AllApplesPresenter.I.Present(gameState.apples);
			WallsPresenter.I.Present(gameState.walls);
		}
	}

	public void Clean() {
		ServerPresenter.I.Clean();
		playerCount = 0;

		if (Client.isClient) {
			AllSnakesPresenter.I.Clean();
			AllApplesPresenter.I.Clean();
			WallsPresenter.I.Clean();
		}
	}
}
