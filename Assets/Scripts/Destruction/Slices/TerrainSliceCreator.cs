using System.Collections.Generic;
using UnityEngine;

public class TerrainSliceCreator : SliceCreator
{
    private static int color = 0;
    private static int change = 2;

    public override void CreateSlice(Transform original, MeshContainer slice, Mesh simplifiedColliderMesh, Vector3 forceDirection, SlicePhysicsProperties slicePhysicsProperties)
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

        if (color != 0)
        {
            List<Color> vertexColors = new List<Color>();
            for (int i = 0; i < mesh.vertexCount; i++)
            {
                if (color == 1)
                {
                    vertexColors.Add(Color.red);
                }
                else
                {
                    vertexColors.Add(Color.blue);
                }
            }
            change--;

            if (color == 1 && change==0)
            {
                color = 2;
                change = 2;
            }

            if(color==2 && change==0)
            {
                color = 1;
                change = 2;
            }


            mesh.SetColors(vertexColors);
        }
      
//		mesh.SetUVs(0, slice.Uvs);		
		mesh.RecalculateNormals();		
		
		MeshRenderer renderer = newSlice.AddComponent<MeshRenderer>();
		MeshFilter filter = newSlice.AddComponent<MeshFilter>();

		filter.mesh = mesh;
        
		renderer.material = Instantiate(GetComponent<MeshRenderer>().material);
        
		filter.sharedMesh.RecalculateBounds();

		MeshCollider collider = newSlice.AddComponent<MeshCollider>();
		collider.material = slicePhysicsProperties.physicMaterial;
	
		Rigidbody rigidbody = newSlice.AddComponent<Rigidbody>();
		rigidbody.mass = 1000;
		rigidbody.useGravity = false;
		rigidbody.isKinematic = true;

		newSlice.AddComponent<TerrainSliceCreator>();
		newSlice.AddComponent<PlaneCutTest>();
		newSlice.AddComponent<FlatMeshMerger>();

		GameObject g = new GameObject(original.gameObject.name+" - slice");
		g.layer = LayerMask.NameToLayer("TerrainPhysics");
		
		g.transform.position = reference.position;
		g.transform.rotation = reference.rotation;
		g.transform.localScale =reference.localScale;

        MeshCollider convexMeshCollider = g.AddComponent<MeshCollider>();
        convexMeshCollider.sharedMesh = simplifiedColliderMesh;
        convexMeshCollider.convex = true;

        Rigidbody parentRigidBody = g.AddComponent<Rigidbody>();
		parentRigidBody.useGravity = false;
	
		parentRigidBody.mass = AssignMass(renderer, slicePhysicsProperties.baseMass);
        if (parentRigidBody.mass < 100)
        {
            newSlice.AddComponent<DissolveObjectWithParent>();
        }
		parentRigidBody.drag = slicePhysicsProperties.drag;
		parentRigidBody.angularDrag = slicePhysicsProperties.angularDrag;
		parentRigidBody.constraints = slicePhysicsProperties.constraints;
		parentRigidBody.AddForce(forceDirection*slicePhysicsProperties.baseMass*10, ForceMode.Impulse);

		newSlice.transform.parent = g.transform;
    }

    private float AssignMass(Renderer renderer, float modifier)
    {
	    Vector3 size = renderer.bounds.size;
	    return size.x * size.y * size.z * modifier;
    }
}
