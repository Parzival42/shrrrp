using UnityEngine;

public abstract class SliceCreator : MonoBehaviour
{
    public void CreateSlice(Transform original, MeshContainer slice, Mesh simpliefiedColliderMesh, Vector3 forceDirection, SlicePhysicsProperties slicePhysicsProperties)
    {
        CreateSlice(original, slice, simpliefiedColliderMesh, forceDirection, slicePhysicsProperties, false);
    }
    
    public abstract void CreateSlice(Transform original, MeshContainer slice, Mesh simpliefiedColliderMesh, Vector3 forceDirection, SlicePhysicsProperties slicePhysicsProperties, bool dissolve);

    /// <summary>
    /// sets the mass according to the properties of the SlicePhysicsProperties object
    /// </summary>
    /// <returns>true if a mass was set successfully, false if slice is to small and thus needs to be destroyed</returns>
    protected bool CalculateMass(Rigidbody rigidbody, Renderer renderer, Vector3 scale, SlicePhysicsProperties slicePhysicsProperties)
    {
        Vector3 size = renderer.bounds.size;
        float ratio = size.x * scale.x * size.y * scale.y * size.z * scale.z / slicePhysicsProperties.referenceVolume;

        if (!slicePhysicsProperties.useThresholds)
        {
            if (ratio < slicePhysicsProperties.minVolume)
            {
                return false;
            }
            rigidbody.mass = ratio * slicePhysicsProperties.baseMass;
            return true;
        }

        for (int i = 0; i < slicePhysicsProperties.volumeThresholds.Length; i++)
        {
            if (ratio >= slicePhysicsProperties.volumeThresholds[i])
            {
                rigidbody.mass = slicePhysicsProperties.massThresholds[i];
                return true;
            }
        }

        return false;
    }
}