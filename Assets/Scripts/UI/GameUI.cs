using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour {
	public static GameUI I;

	public GameObject mainMenu;

	void Awake() {
		I = this;
	}

	void Update() {

	}

	public void ToggleMainMenu(bool active) {
		mainMenu.SetActive(active);
	}
}
