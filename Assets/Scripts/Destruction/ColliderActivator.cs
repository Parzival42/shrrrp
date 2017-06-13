using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderActivator : MonoBehaviour {
	void Start () {
		StartCoroutine(WaitForActivation(0.3f));
	}
	
	private IEnumerator WaitForActivation(float delay){
		//yield return new WaitForSeconds(delay);
		yield return new WaitForFixedUpdate();
		Collider collider = GetComponent<Collider>();
		if(collider!=null){
			GetComponent<Collider>().enabled = true;
		}
	}
}
