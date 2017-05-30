using UnityEngine;
using System.Collections;

public class DissolveObjectWithDelay : DissolveObject {

	void Start () {
		StartCoroutine("DelayedDissolve", Random.Range(1.3f, 2.0f));
	}

	private IEnumerator DelayedDissolve(float delay){
		yield return new WaitForSeconds(delay);
		PrepareDissolve(1.0f);
	}
}
