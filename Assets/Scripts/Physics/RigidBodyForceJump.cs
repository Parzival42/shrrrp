using UnityEngine;

/// <summary>
/// Handles forces on RigidBody's when the player jumps.
/// The jump wields force on the detected RigidBody under the player.
/// </summary>
[RequireComponent(typeof(RigidBodyInput))]
public class RigidBodyForceJump : MonoBehaviour
{
    #region Inspector variables

    [FancyHeader("Force settings")] [SerializeField] private float pushAwayForce = 10f;

    [SerializeField] private ForceMode forceMode = ForceMode.VelocityChange;

    [FancyHeader("Collision settings")] [SerializeField] private LayerMask layerMask;

    #endregion

    #region Internal Members

    private RigidBodyInput rigidBodyInput;

    #endregion

    private void Start()
    {
        rigidBodyInput = GetComponent<RigidBodyInput>();
        rigidBodyInput.InputController.OnJump += HandleJump;
    }

    private void HandleJump()
    {
        RaycastHit hitInfo;
        bool hitGround = CheckGround(out hitInfo);

        if (hitGround)
            ApplyForce(hitInfo);
    }

    private void ApplyForce(RaycastHit hitInfo)
    {
        Rigidbody rigid = hitInfo.transform.parent.GetComponent<Rigidbody>();
        Collider coll = hitInfo.transform.gameObject.GetComponent<Collider>();
        if (rigid != null && coll != null)
        {
            rigid.AddForceAtPosition(Vector3.down * pushAwayForce, coll.ClosestPoint(hitInfo.point), forceMode);
        }
    }

    private bool CheckGround(out RaycastHit hitInfo)
    {
        Ray ray = new Ray(
            rigidBodyInput.transform.position + rigidBodyInput.GroundPivotOffset,
            Vector3.down);
        
        return Physics.Raycast(ray, out hitInfo, rigidBodyInput.GroundedDistance, layerMask.value);
    }
}