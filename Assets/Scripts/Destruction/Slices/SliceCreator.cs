using UnityEngine;

public abstract class SliceCreator : MonoBehaviour {

	public abstract void CreateSlice(Transform original, MeshContainer slice, Vector3 forceDirection, SlicePhysicsProperties slicePhysicsProperties);

}
