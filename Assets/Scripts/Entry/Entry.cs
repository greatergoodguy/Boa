using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Entry : MonoBehaviour {
	public static bool isLAN;

	void Start() {
		Debug.Log("COMMAND LINE ARGS: ");
		var args = Environment.GetCommandLineArgs();
		if (HasServerArg(args)) {
			if (HasLanArg(args)) {
				isLAN = true;
			}
			SceneManager.LoadScene("server");
		} else {
			SceneManager.LoadScene("client");
		}
	}

	bool HasServerArg(string[] args) => args.Any(x => x.ToLowerInvariant() == "server");
	bool HasLanArg(string[] args) => args.Any(x => x.ToLowerInvariant() == "lan");

	void Update() {

	}
}
