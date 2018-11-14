using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicidalGene : MonoBehaviour {
	public float secondsToLive;

	void Start() {
		StartCoroutine(DestroySelfAfterNSeconds());
	}

	IEnumerator DestroySelfAfterNSeconds() {
		yield return new WaitForSeconds(secondsToLive);
		Destroy(this.gameObject);
	}
}
