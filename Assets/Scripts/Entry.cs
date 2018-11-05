using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Entry : MonoBehaviour {
	void Start() {
		Debug.Log("COMMAND LINE ARGS: ");
		var args = Environment.GetCommandLineArgs();
		if (HasServerArg(args)) {
			SceneManager.LoadScene("server");
		} else {
			SceneManager.LoadScene("client");
		}
	}

	bool HasServerArg(string[] args) => args.Any(x => x.ToLowerInvariant() == "server");

	void Update() {

	}
}
