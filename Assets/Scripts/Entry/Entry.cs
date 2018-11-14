using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Entry : MonoBehaviour {
	void Start() {
		var args = Environment.GetCommandLineArgs();

		Debug.Log("COMMAND LINE ARGS: " + JsonConvert.SerializeObject(args));

		if (args.Any(x => x.ToLowerInvariant() == "server")) {
			SceneManager.LoadScene("server");
		} else {
			SceneManager.LoadScene("client");
		}
	}
}
