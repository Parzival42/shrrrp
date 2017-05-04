
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSliceCreator : SliceCreator
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
		mesh.RecalculateNormals();

		
		MeshRenderer renderer = newSlice.AddComponent<MeshRenderer>();
		MeshFilter filter = newSlice.AddComponent<MeshFilter>();

		filter.mesh = mesh;
		renderer.material = GetComponent<MeshRenderer>().material;
		filter.sharedMesh.RecalculateBounds();

		MeshCollider collider = newSlice.AddComponent<MeshCollider>();
		//collider.convex = true;
	
		Rigidbody rigidbody = newSlice.AddComponent<Rigidbody>();
		rigidbody.mass = 1000;
		rigidbody.useGravity = false;
		rigidbody.isKinematic = true;


		newSlice.AddComponent<TerrainSliceCreator>();

		newSlice.AddComponent<PlaneCutTest>();
		newSlice.AddComponent<FlatMeshMerger>();
		newSlice.AddComponent<TriangulatorTest>();


    }
}
