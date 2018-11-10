using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AllApplesPresenter : MonoBehaviour, IPresenter<AllApplesState> {
	public static AllApplesPresenter I;

	public GameObject applePrefab;

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
		allApplesState.all.ZipDo(apples, (state, apple) => apple.transform.position = state.position.ToUnityVector2());
	}
}
