using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AllApplesPresenter : MonoBehaviour, IPresenter<AllApplesState> {
	public static AllApplesPresenter I;

	public GameObject applePrefab;
	public GameObject appleExplosionPrefab;

	List<GameObject> apples = new List<GameObject>();

	void Awake() {
		I = this;
	}

	void Update() {

	}

	public void Present(AllApplesState allApplesState) {
		while (apples.Count < allApplesState.all.Length) {
			apples.Add(Instantiate(applePrefab, Vector3.zero, Quaternion.identity));
		}

		while (apples.Count > allApplesState.all.Length) {
			Destroy(apples.Last());
			apples.RemoveAt(apples.Count - 1);
		}

		foreach (var eatenApple in allApplesState.eatenApples) {
			OnAppleEaten(eatenApple);
		}

		allApplesState.all.ZipDo(apples, (state, apple) => apple.transform.position = state.position.ToUnityVector2());
	}

	void OnAppleEaten(DG_Vector2 position) {
		Instantiate(appleExplosionPrefab, position.ToUnityVector2(), Quaternion.identity);
	}

	public void Clean() {
		apples.ForEach(x => Destroy(x));
		apples.Clear();
	}
}
