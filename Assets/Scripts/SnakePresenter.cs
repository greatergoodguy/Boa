using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SnakePresenter : MonoBehaviour, IPresenter<SnakeState> {
	GameObject head;
	List<GameObject> tails = new List<GameObject>();

	void Start() {

	}

	void Update() {

	}

	public void Present(SnakeState snakeState) {
		if (head == null) head = Instantiate(AllSnakesPresenter.I.snakePrefab, Vector3.zero, Quaternion.identity);
		head.transform.position = snakeState.headPosition.ToUnityVector2();

		while (tails.Count < snakeState.tails.Length) {
			tails.Add(Instantiate(AllSnakesPresenter.I.snakePrefab, Vector3.zero, Quaternion.identity));
		}
		tails.ZipDo(snakeState.tails, (tail, state) => tail.transform.transform.position = state.ToUnityVector2());
	}
}
