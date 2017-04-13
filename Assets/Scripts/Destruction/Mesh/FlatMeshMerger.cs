using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatMeshMerger : MonoBehaviour, MeshMerger {
    public MeshContainer Merge(MeshContainer recipient, MeshContainer mergee)
    {
		int lastIndex = recipient.Vertices.Count;

		for(int i = 0; i < mergee.Vertices.Count; i++){
			recipient.Vertices.Add(mergee.Vertices[i]);
			recipient.Normals.Add(i < mergee.Normals.Count ? mergee.Normals[i] : new Vector3());
			recipient.Uvs.Add(i < mergee.Uvs.Count ? mergee.Uvs[i] : new Vector2());
		}

		if(recipient.Indices.Length != mergee.Indices.Length){
			throw new Exception();
		}
		
		

		for(int i = 0; i < mergee.Indices.Length; i++){
			for(int j = 0; j < mergee.Indices[i].Count; j++){
				recipient.Indices[i].Add(mergee.Indices[i][j]+lastIndex);
			}
		}

		return recipient;
    } 
}
