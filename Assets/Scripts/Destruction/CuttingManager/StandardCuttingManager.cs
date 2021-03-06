﻿using System.Collections;
using System.Collections.Generic;
using CielaSpike;
using UnityEngine;

public class StandardCuttingManager : MonoBehaviour, CuttingManager {

	#region variables
	[SerializeField]
	private bool isMenu;

	[SerializeField]
	private bool suddenDeath;

	[SerializeField]
	private LayerMask layerMask;
	[SerializeField]
	private SlicePhysicsProperties sliceProperties;
	#endregion 


	public void Start(){
		CuttingManagerLocator.Provide(this);
	}

	public void Cut(Transform planeTransform, Mesh planeMesh){
		Cut(planeTransform, planeMesh, 0.0f);
	}

	public void Cut(Transform planeTransform, Mesh planeMesh, float delay){
		Collider[] intersectingColliders = Physics.OverlapBox(planeTransform.position, new Vector3(50, 0.0001f, 50), planeTransform.rotation, layerMask);

		Plane cuttingPlane = new Plane();
		cuttingPlane.SetNormalAndPosition(planeTransform.position, planeTransform.up);
		cuttingPlane.Set3Points(planeTransform.TransformPoint(planeMesh.vertices[planeMesh.triangles[0]]),
			planeTransform.TransformPoint(planeMesh.vertices[planeMesh.triangles[1]]),
			planeTransform.TransformPoint(planeMesh.vertices[planeMesh.triangles[2]]));

		for(int i = 0; i < intersectingColliders.Length; i++){	
			GameObject go = intersectingColliders[i].gameObject;
			if(!go.CompareTag("CuttingPlane")){
				continue;
			}

			CuttingLogic pct = go.GetComponent<CuttingLogic>();
			if(pct==null){
				if(!isMenu){
					go.AddComponent<TerrainSliceCreator>();
				}else{
					go.AddComponent<MenuLetterSliceCreator>();
				}
				go.AddComponent<FlatMeshMerger>();
				pct = go.AddComponent<CuttingLogic>();				
			}
			
			
			Task task;
			this.StartCoroutineAsync(pct.CuttingCoroutine(cuttingPlane, new MeshContainer(go.GetComponent<MeshFilter>().mesh, true), sliceProperties, suddenDeath, delay), out task);
		}
	}
}
