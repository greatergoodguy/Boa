using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AllSnakesPresenter : MonoBehaviour, IPresenter<AllSnakesState> {
	public static AllSnakesPresenter I;

	public SnakePresenter[] snakePresenters;

	void Awake() {
		I = this;
	}

	void Update() {

	}

	public void Present(AllSnakesState gameState) {
		snakePresenters.ZipDo(gameState.all, (x, y) => x.Present(y));
	}
}
