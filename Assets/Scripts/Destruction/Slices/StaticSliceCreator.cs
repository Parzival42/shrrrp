using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticSliceCreator : SliceCreator
{
    public override void CreateSlice(Transform original, MeshContainer slice)
    {
        GameObject newSlice = new GameObject(original.gameObject.name+" - slice");
		
		newSlice.transform.position = original.position;
		newSlice.transform.rotation = original.rotation;
		newSlice.transform.localScale = original.localScale;

		Mesh mesh = new Mesh();
		mesh.SetVertices(slice.Vertices);

		mesh.subMeshCount = slice.Indices.Length;
		for (int i = 0; i < slice.Indices.Length; i++)
		{
			mesh.SetIndices(slice.Indices[i].ToArray(), MeshTopology.Triangles, i);
		}

		mesh.SetNormals(slice.Normals);
		mesh.SetUVs(0, slice.Uvs);
		// if(slice.Uvs!=null && slice.Uvs.Count>0){
		// 	mesh.SetUVs(0, slice.Uvs);
		// }else{
		// 	List<Vector2> uvs = new List<Vector2>();
		// 	for(int i = 0; i < slice.Vertices.Count; i++){
		// 		uvs[i] = new Vector2();
		// 	}
		// 	mesh.SetUVs(0,uvs );
		// }
		mesh.RecalculateNormals();

		
		MeshRenderer renderer = newSlice.AddComponent<MeshRenderer>();
		MeshFilter filter = newSlice.AddComponent<MeshFilter>();

		filter.mesh = mesh;
		renderer.material = GetComponent<MeshRenderer>().material;
		filter.sharedMesh.RecalculateBounds();
    }
}
