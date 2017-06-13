using UnityEngine;

public class MenuLetterSliceCreator : SliceCreator {
    public override void CreateSlice(Transform original, MeshContainer slice, Mesh simplifiedColliderMesh, Vector3 forceDirection, SlicePhysicsProperties slicePhysicsProperties)
    {
        GameObject newSlice = new GameObject(original.gameObject.name+" - slice");
		Transform reference = original.parent;
		if(reference == null){
			reference = original;
		}
		
		newSlice.transform.position = original.position;
		newSlice.transform.rotation = original.rotation;
		newSlice.transform.localScale = original.localScale;
		newSlice.layer = LayerMask.NameToLayer("Default");

		Mesh mesh = new Mesh();
		mesh.SetVertices(slice.Vertices);

		mesh.subMeshCount = slice.Indices.Length;
		for (int i = 0; i < slice.Indices.Length; i++)
		{
			mesh.SetIndices(slice.Indices[i].ToArray(), MeshTopology.Triangles, i);
		}

		mesh.SetNormals(slice.Normals);
		mesh.RecalculateNormals();		
		
		MeshRenderer renderer = newSlice.AddComponent<MeshRenderer>();
		MeshFilter filter = newSlice.AddComponent<MeshFilter>();

		filter.mesh = mesh;
        
		renderer.material = Instantiate(GetComponent<MeshRenderer>().material);
		filter.sharedMesh.RecalculateBounds();

		Rigidbody rigidbody = newSlice.AddComponent<Rigidbody>();
		
		rigidbody.mass = 100;
		rigidbody.angularDrag = Random.Range(0.1f, 1.0f);
		rigidbody.drag = Random.Range(0.1f, 1.0f);
		rigidbody.isKinematic = false;
		rigidbody.useGravity = false;
		rigidbody.AddForce(forceDirection*Random.Range(50,500), ForceMode.Impulse);
		rigidbody.AddTorque(transform.right*Random.Range(-0.3f,0.3f), ForceMode.Impulse);

		newSlice.transform.parent = original.parent;
		newSlice.AddComponent<DissolveMenuObjectWithDelay>();
    }
}
