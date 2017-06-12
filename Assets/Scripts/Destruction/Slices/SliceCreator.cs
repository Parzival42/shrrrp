using UnityEngine;

public abstract class SliceCreator : MonoBehaviour {

	public abstract void CreateSlice(Transform original, MeshContainer slice, Mesh simpliefiedColliderMesh, Vector3 forceDirection, SlicePhysicsProperties slicePhysicsProperties);

}
