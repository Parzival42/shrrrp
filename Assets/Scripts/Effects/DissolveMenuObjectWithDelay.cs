using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveMenuObjectWithDelay : DissolveObject {

	void Start () {
		StartCoroutine("DelayedDissolve", Random.Range(1.0f, 1.6f));
	}

	private IEnumerator DelayedDissolve(float delay){
		yield return new WaitForSeconds(delay);
		PrepareDissolve(1.0f, "DissolveMaterialMenu");
	}
}
