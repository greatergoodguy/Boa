using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AllSnakesPresenter : MonoBehaviour, IPresenter<AllSnakesState> {
	public static AllSnakesPresenter I;

	public GameObject snakeHeadPrefab;
	public GameObject snakeTailPrefab;

	public Dictionary<int, SnakePresenter> snakePresenters = new Dictionary<int, SnakePresenter>();

	void Awake() {
		I = this;
	}

	void Update() {

	}

	public void Present(AllSnakesState allSnakesState) {
		// Add and Present
		foreach (var snakeState in allSnakesState.all) {
			if (snakePresenters.ContainsKey(snakeState.ownerId) == false) {
				snakePresenters[snakeState.ownerId] = new GameObject().AddComponent<SnakePresenter>();
			}
			snakePresenters[snakeState.ownerId].Present(snakeState);
		}

		// Remove
		var toRemove = new List<int>();
		foreach (var kvp in snakePresenters) {
			if (allSnakesState.all.Any(x => x.ownerId == kvp.Key) == false) {
				kvp.Value.Clean();
				toRemove.Add(kvp.Key);
			}
		}
		foreach (var item in toRemove) {
			snakePresenters.Remove(item);
		}
	}

	public void Clean() {
		foreach (var snakePresenter in snakePresenters.Values) {
			snakePresenter.Clean();
		}
		snakePresenters.Clear();
	}
}
