using UnityEngine;

public class TerrainSliceCreator : SliceCreator
{
    public override void CreateSlice(Transform original, MeshContainer slice, Mesh simplifiedColliderMesh, Vector3 forceDirection, SlicePhysicsProperties slicePhysicsProperties, bool dissolve)
    {
        GameObject newSlice = new GameObject(original.gameObject.name+" - slice");
		Transform reference = original.parent;
		if(reference == null){
			reference = original;
		}
		
		newSlice.transform.position = reference.position;
		newSlice.transform.rotation = reference.rotation;
		newSlice.transform.localScale = reference.localScale;
		newSlice.layer = LayerMask.NameToLayer("Ground");
		newSlice.tag = "CuttingPlane";

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

		MeshCollider collider = newSlice.AddComponent<MeshCollider>();
		collider.material = slicePhysicsProperties.physicMaterial;
	
		Rigidbody rigidbody = newSlice.AddComponent<Rigidbody>();
		rigidbody.isKinematic = true;

		newSlice.AddComponent<TerrainSliceCreator>();
		newSlice.AddComponent<PlaneCutTest>();
		newSlice.AddComponent<FlatMeshMerger>();

		GameObject g = new GameObject(original.gameObject.name+" - slice");
		g.layer = LayerMask.NameToLayer("TerrainPhysics");
		
		g.transform.position = reference.position;
		g.transform.rotation = reference.rotation;
		g.transform.localScale =reference.localScale;

    
        Rigidbody parentRigidBody = g.AddComponent<Rigidbody>();
		parentRigidBody.useGravity = false;

	    if (CalculateMass(parentRigidBody, renderer, newSlice.transform.localScale, slicePhysicsProperties) && !dissolve)
	    {
		    parentRigidBody.drag = slicePhysicsProperties.drag;
		    parentRigidBody.angularDrag = slicePhysicsProperties.angularDrag;
		    parentRigidBody.AddForce(Vector3.ProjectOnPlane(forceDirection, Vector3.up).normalized*slicePhysicsProperties.cuttingForce, ForceMode.Impulse);
	        
		    MeshCollider convexMeshCollider = g.AddComponent<MeshCollider>();
		    convexMeshCollider.sharedMesh = simplifiedColliderMesh;
		    convexMeshCollider.convex = true;
		    convexMeshCollider.enabled = false;
	    }
	    else
	    {
		    parentRigidBody.isKinematic = true;
		    newSlice.AddComponent<DissolveObjectWithParent>();
	    }
	  
		g.AddComponent<ColliderActivator>();
		newSlice.transform.parent = g.transform;
    }
}
