using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingManager : MonoBehaviour {

	private static CuttingManager cuttingManagerInstance;

	public static CuttingManager CuttingManagerInstance{
		get{
			cuttingManagerInstance = GameObject.FindObjectOfType<CuttingManager>();

			if(cuttingManagerInstance == null){
				GameObject manager = new  GameObject("Cutting Manager");
				cuttingManagerInstance = manager.AddComponent<CuttingManager>();
				DontDestroyOnLoad(cuttingManagerInstance);
			}

			return cuttingManagerInstance;
		}
	}

	public void Cut(Transform planeTransform, Mesh planeMesh){
		Collider[] intersectingColliders = Physics.OverlapBox(planeTransform.position, new Vector3(50, 0.0001f, 50), planeTransform.rotation);

		

		Debug.Log("intersection count: "+intersectingColliders.Length);

		Plane cuttingPlane = new Plane();
		cuttingPlane.SetNormalAndPosition(planeTransform.position, planeTransform.up);
		cuttingPlane.Set3Points(planeTransform.TransformPoint(planeMesh.vertices[planeMesh.triangles[0]]),
			planeTransform.TransformPoint(planeMesh.vertices[planeMesh.triangles[1]]),
			planeTransform.TransformPoint(planeMesh.vertices[planeMesh.triangles[2]]));
		DebugExtension.DebugArrow(transform.position, transform.up, Color.red, 10.0f);

		for(int i = 0; i < intersectingColliders.Length; i++){	
			GameObject go = intersectingColliders[i].gameObject;
			if(go.tag != "CuttingPlane"){
				continue;
			}
			Debug.Log("Processing "+go.name);
			PlaneCutTest pct = go.GetComponent<PlaneCutTest>();
			if(pct==null){
				go.AddComponent<TerrainSliceCreator>();
				go.AddComponent<TriangulatorTest>();
				go.AddComponent<FlatMeshMerger>();
				pct = go.AddComponent<PlaneCutTest>();				
			}

			Debug.Log("cutting: "+ go.name);
			pct.StartSplitInTwo(cuttingPlane, true);
		}
	}
}
