using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AllSnakesPresenter : MonoBehaviour, IPresenter<AllSnakesState> {
	public static AllSnakesPresenter I;

	public GameObject snakePrefab;

	List<GameObject> snakes = new List<GameObject>();

	void Awake() {
		I = this;
	}

	void Update() {

	}

	public void Present(AllSnakesState gameState) {
		while (snakes.Count < gameState.all.Length) {
			snakes.Add(Instantiate(snakePrefab, Vector3.zero, Quaternion.identity));
		}
		gameState.all.ZipDo(snakes, (state, snake) => snake.transform.position = state.position.ToUnityVector2());
	}
}
