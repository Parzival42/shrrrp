using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardCuttingManager : MonoBehaviour, CuttingManager {

	#region variables
	[SerializeField]
	private LayerMask layerMask;
	[SerializeField]
	private SlicePhysicsProperties sliceProperties;
	#endregion 


	public void Start(){
		CuttingManagerLocator.Provide(this);
	}

	public void Cut(Transform planeTransform, Mesh planeMesh){
		Collider[] intersectingColliders = Physics.OverlapBox(planeTransform.position, new Vector3(50, 0.0001f, 50), planeTransform.rotation, layerMask);

		Plane cuttingPlane = new Plane();
		cuttingPlane.SetNormalAndPosition(planeTransform.position, planeTransform.up);
		cuttingPlane.Set3Points(planeTransform.TransformPoint(planeMesh.vertices[planeMesh.triangles[0]]),
			planeTransform.TransformPoint(planeMesh.vertices[planeMesh.triangles[1]]),
			planeTransform.TransformPoint(planeMesh.vertices[planeMesh.triangles[2]]));

		for(int i = 0; i < intersectingColliders.Length; i++){	
			GameObject go = intersectingColliders[i].gameObject;
			if(go.tag != "CuttingPlane"){
				continue;
			}

			PlaneCutTest pct = go.GetComponent<PlaneCutTest>();
			if(pct==null){
				go.AddComponent<TerrainSliceCreator>();
				go.AddComponent<TriangulatorTest>();
				go.AddComponent<FlatMeshMerger>();
				pct = go.AddComponent<PlaneCutTest>();				
			}

			pct.StartSplitInTwo(cuttingPlane, true, sliceProperties);
		}
	}
}
