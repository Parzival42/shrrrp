using UnityEngine;

/// <summary>
/// Basic abstract force field class. Inherit from this class
/// in order to implement specific ForceField implementations.
/// </summary>
public abstract class ForceField : MonoBehaviour
{
    #region Inspector variables
    [FancyHeader("General settings", "General force field settings")]
    [SerializeField]
    private float forceMultiplier = 1f;

    [SerializeField]
    private ForceMode forceMode = ForceMode.VelocityChange;

    [SerializeField]
    private bool debugDraw = false;
    #endregion

    #region Abstract methods
    /// <summary>
    /// Calculates the force vector based on the given position. This calculation
    /// defines the shape of the force field.
    /// </summary>
    protected abstract Vector3 GetForceFor(Vector3 position);

    /// <summary>
    /// Visualizes the forces.
    /// </summary>
    protected abstract void DrawDebugVisualization();
    #endregion

    #region Methods
#if UNITY_EDITOR
    /// <summary>
    /// Unity MonoBehaviour Update callback.
    /// </summary>
    private void Update()
    {
        if(debugDraw)
            DrawDebugVisualization();
    }
#endif

    /// <summary>
    /// Applies a force based on the 'GetForceFor(position)' method for the 
    /// given RigidBody.
    /// </summary>
    protected virtual void ApplyForce(Rigidbody rigid)
    {
        Vector3 force = GetForceFor(rigid.position);
        rigid.AddForce(force * forceMultiplier, forceMode);
    }

    /// <summary>
    /// Callback of MonoBehaviour. Applies force on the attached RigidBody.
    /// </summary>
    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody)
            ApplyForce(other.attachedRigidbody);
    }
    #endregion
}