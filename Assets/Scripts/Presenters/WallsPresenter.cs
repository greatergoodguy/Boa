using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WallsPresenter : MonoBehaviour, IPresenter<DG_Vector2[]> {
	public static WallsPresenter I;

	public GameObject wallPrefab;

	List<GameObject> walls = new List<GameObject>();

	void Awake() {
		I = this;
	}

	void Update() {

	}

	public void Present(DG_Vector2[] wallsState) {
		while (walls.Count < wallsState.Length) {
			walls.Add(Instantiate(wallPrefab, Vector3.zero, Quaternion.identity));
		}

		while (walls.Count > wallsState.Length) {
			Destroy(walls.Last());
			walls.RemoveAt(walls.Count - 1);
		}

		wallsState.ZipDo(walls, (state, wall) => wall.transform.position = state.ToUnityVector2());
	}

	public void Clean() {
		walls.ForEach(x => Destroy(x));
		walls.Clear();
	}
}
