using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshSplitter : MonoBehaviour {

#region variables

	[SerializeField]
	private GameObject referencePlane;

	[SerializeField]
	private bool cut;

	private bool first = true;

	private Plane cuttingPlane;

	private MeshFilter meshFilter;

	private Mesh mesh;

	private Mesh pM;

	private Transform pT;

	private List<int> leftIndices = new List<int>();

	private List<int> rightIndices = new List<int>();

#endregion


#region methods

	void Start () {
		meshFilter = GetComponent<MeshFilter>(); 
		mesh = meshFilter.mesh;		
	}
	
	// Update is called once per frame
	void Update () {
		if(first && cut){
			SplitInTwo();
			cut = false;
		}
	}

    private void SplitInTwo()
    {
		float startTime = Time.realtimeSinceStartup;

		Mesh pM = referencePlane.GetComponent<MeshFilter>().mesh;
		Transform pT = referencePlane.transform;
		cuttingPlane.Set3Points(pT.TransformPoint(pM.vertices[pM.triangles[0]]), pT.TransformPoint(pM.vertices[pM.triangles[1]]),pT.TransformPoint(pM.vertices[pM.triangles[2]]));

		leftIndices = new List<int>();
		rightIndices = new List<int>();

		for(int i = 0; i < mesh.vertexCount; i++){
			if(cuttingPlane.GetSide(transform.TransformPoint(mesh.vertices[i]))){
				leftIndices.Add(i);
			}else{
				rightIndices.Add(i);
			}
		}

		float duration = Time.realtimeSinceStartup - startTime;
		Debug.Log("vertex sorting time: "+duration);
		Debug.Log("left vertex count: "+leftIndices.Count+"   right vertex count: "+rightIndices.Count);
    }


	private void DetermineConflictTriangles(Plane cuttingPlane, Mesh mesh){
		int[] triangles = mesh.triangles;

		for(int i = 0; i < triangles.Length; i+=3){
				


		}


	}

#endregion


#region debug stuff
	void OnDrawGizmos(){
		Gizmos.color = Color.green;
		for(int i = 0; i < leftIndices.Count; i++){
			Gizmos.DrawCube(transform.TransformPoint(mesh.vertices[leftIndices[i]]), new Vector3(0.05f, 0.05f, 0.05f));
		}
	}
#endregion




}
