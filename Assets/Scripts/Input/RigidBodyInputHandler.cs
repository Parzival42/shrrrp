using UnityEngine;

public class RigidBodyInputHandler : InputHandler
{
    #region Constants
    private static float MOVE_EPSILON = 0.1f;
    private static string HORIZONTAL_AXIS = "Horizontal";
    private static string VERTICAL_AXIS = "Vertical";
    private static string JUMP_AXIS = "Jump";
    #endregion

    #region Internal members
    private RigidBodyInput player;

    // Due to performance reasons this variable is declared here
    private Vector3 movementVector = Vector3.zero;
    private bool movementEnabled = true;
    #endregion

    #region Properties
    public bool MovementEnabled
    {
        get { return movementEnabled; }
        set { movementEnabled = value; }
    }
    #endregion

    public RigidBodyInputHandler(RigidBodyInput player)
    {
        this.player = player;
    }

    /// <summary>
    /// Should be called from an update loop.
    /// </summary>
    public void HandleInput()
    {
        if (MovementEnabled)
        {
            CheckInput();
        }
    }

    private void CheckInput()
    {
        float horizontal = Input.GetAxis(HORIZONTAL_AXIS);
        float vertical = Input.GetAxis(VERTICAL_AXIS);
        movementVector.Set(0f, 0f, 0f);

        // Horizontal Axis
        HandleAxisInput(horizontal, Vector3.right);

        // Vertical Axis
        HandleAxisInput(vertical, Vector3.forward);

        // Handle jumping
        HandleJumpInput();

        // Apply movement and rotation
        ApplyMovement(movementVector);
        ApplyRotation(horizontal, vertical);
    }

    private void HandleJumpInput()
    {
        bool hitGround = CheckPlayerGrounded();
        if (hitGround && Input.GetButtonDown(JUMP_AXIS))
        {
            player.Rigid.AddForce(Vector3.up * player.JumpIntensity, ForceMode.VelocityChange);
        }
    }

    private void HandleAxisInput(float axis, Vector3 direction)
    {
        if (axis > MOVE_EPSILON)
            movementVector += player.MovementSpeed * Mathf.Abs(axis) * direction;
        else if (axis < MOVE_EPSILON)
            movementVector += player.MovementSpeed * Mathf.Abs(axis) * -direction;
    }

    private void ApplyRotation(float horizontalInput, float verticalInput)
    {
        if (Mathf.Abs(horizontalInput) > MOVE_EPSILON || Mathf.Abs(verticalInput) > MOVE_EPSILON)
        {
            float yAngle = Mathf.Atan2(horizontalInput, verticalInput) * Mathf.Rad2Deg;

            player.Rigid.MoveRotation(
                Quaternion.Lerp(player.transform.rotation,
                Quaternion.Euler(0f, yAngle, 0f),
                Time.deltaTime * player.RotationSpeed));
        }
    }

    private void ApplyMovement(Vector3 movementVector)
    {
        player.Rigid.MovePosition(player.transform.position + movementVector);
    }

    private void ApplyMovement(float movementSpeed, Vector3 direction)
    {
        ApplyMovement(direction * movementSpeed);
    }

    private bool CheckPlayerGrounded()
    {
        Ray ray = new Ray(player.transform.position + player.GroundPivotOffset, Vector3.down);
        RaycastHit hitInfo;
        bool hitGround = Physics.Raycast(ray, out hitInfo, player.GroundedDistance, 1 << player.GroundedLayer, QueryTriggerInteraction.UseGlobal);

#if UNITY_EDITOR
        if (hitGround)
            Debug.DrawRay(ray.origin, ray.direction * player.GroundedDistance, Color.green);
        else
            Debug.DrawRay(ray.origin, ray.direction * player.GroundedDistance, Color.red);
#endif

        return hitGround;
    }
}