using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AllSnakesPresenter : MonoBehaviour, IPresenter<AllSnakesState> {
	public static AllSnakesPresenter I;

	public GameObject snakePrefab;

	public Dictionary<int, SnakePresenter> snakePresenters = new Dictionary<int, SnakePresenter>();

	void Awake() {
		I = this;
	}

	void Update() {

	}

	public void Present(AllSnakesState allSnakesState) {
		foreach (var snakeState in allSnakesState.all) {
			if (snakePresenters.ContainsKey(snakeState.ownerId) == false) {
				snakePresenters[snakeState.ownerId] = new GameObject().AddComponent<SnakePresenter>();
			}
			snakePresenters[snakeState.ownerId].Present(snakeState);
		}
	}

	public void Clean() {
		foreach (var snakePresenter in snakePresenters.Values) {
			snakePresenter.Clean();
		}
		snakePresenters.Clear();
	}
}
