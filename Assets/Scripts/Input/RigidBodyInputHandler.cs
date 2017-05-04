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
    private Vector2 normalizedInput = Vector2.zero;
    private Camera mainCamera;

    private float horizontalInputValue = 0f;
    private float verticalInputValue = 0f;

    private Vector3 forwardVector = Vector3.forward;
    private Vector3 rightVector = Vector3.right;

    private bool firstJump = false;
    private bool secondJump = false;
    private bool hitGround = true;
    #endregion

    #region Properties
    public float HorizontalInputValue
    {
        get { return horizontalInputValue; }
        set { horizontalInputValue = value; }
    }

    public float VerticalInputValue
    {
        get { return verticalInputValue; }
        set { verticalInputValue = value; }
    }

    public bool IsGrounded { get { return hitGround; } }
    #endregion

    #region Events
    public event JumpPerformedHandler OnJump;
    public event SecondJumpPerformedHandler OnSecondJump;
    public event LandedOnGroundHandler OnLandedOnGround;
    public event SpecialMoveHandler OnPlayerCut;
    #endregion

    public RigidBodyInputHandler(RigidBodyInput player)
    {
        this.player = player;
        this.mainCamera = CameraUtil.GetMainCamera();
        this.OnLandedOnGround += ResetJumpStateOnLanding;
    }

    /// <summary>
    /// Should be called from an update loop.
    /// </summary>
    public void HandleInput()
    {
        // Set input values regardless if movement is allowed or not
        CalculateInputValues();

        if (player.AllowMovement)
            CheckInput();
    }

    private void CalculateInputValues()
    {
        normalizedInput.Set(player.PlayerAction.Move.X, player.PlayerAction.Move.Y);
        normalizedInput.Normalize();
        HorizontalInputValue = normalizedInput.x;
        VerticalInputValue = normalizedInput.y;
        hitGround = CheckPlayerGrounded();
    }

    private void CheckInput()
    {
        movementVector.Set(0f, 0f, 0f);

        // Calculate forward and right vector
        CalculateDirectionVectors();

        // Horizontal Axis
        HandleAxisInput(HorizontalInputValue, rightVector);

        // Vertical Axis
        HandleAxisInput(VerticalInputValue, forwardVector);

        // Handle jumping
        HandleJumpInput();

        // Apply movement and rotation
        ApplyMovement(movementVector);
        ApplyRotation(HorizontalInputValue, VerticalInputValue);
    }

    private void HandleJumpInput()
    {
        bool jumpButtonDown = player.PlayerAction.Jump.WasPressed;
        

        // Normal ground jump
        if (!firstJump && jumpButtonDown)
        {
            PerformJumpForce(Vector3.up * player.JumpIntensity);
            SetJumpState(true, false);
            OnJumped();
        }
        else if (firstJump && !secondJump && jumpButtonDown)   // Second jump
        {
            PerformJumpForce(Vector3.up * player.JumpIntensity * player.SecondJumpIntensityFactor);
            SetJumpState(true, true);
            OnSecondJumped();
        }
        // Jump state is reseted per event in 'ResetJumpStateOnLanding'.
    }

    private void ResetJumpStateOnLanding()
    {
        SetJumpState(false, false);
    }

    private void SetJumpState(bool firstJump, bool secondJump)
    {
        this.firstJump = firstJump;
        this.secondJump = secondJump;
    }

    private void PerformJumpForce(Vector3 forceVector)
    {
        player.Rigid.velocity = Vector3.zero;       // :(
        player.Rigid.AddForce(forceVector, ForceMode.VelocityChange);
    }

    private void HandleAxisInput(float axis, Vector3 direction)
    {
        if (axis > MOVE_EPSILON)
            movementVector += player.MovementSpeed * Mathf.Abs(axis) * direction;
        else if (axis < MOVE_EPSILON)
            movementVector += player.MovementSpeed * Mathf.Abs(axis) * -direction;
    }

    private void CalculateDirectionVectors()
    {
        if (player.CameraBasedControl)
        {
            forwardVector = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up).normalized;
            rightVector = Vector3.Cross(Vector3.up, forwardVector).normalized;
        }
        else
        {
            forwardVector = Vector3.forward;
            rightVector = Vector3.right;
        }
    }

    private void ApplyRotation(float horizontalInput, float verticalInput)
    {
        if (Mathf.Abs(horizontalInput) > MOVE_EPSILON || Mathf.Abs(verticalInput) > MOVE_EPSILON)
        {
            Quaternion lookAtRotation = Quaternion.LookRotation(movementVector, Vector3.up);

            player.Rigid.MoveRotation(
                Quaternion.Lerp(player.transform.rotation,
                lookAtRotation,
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
        bool groundWasHit = Physics.Raycast(ray, out hitInfo, player.GroundedDistance, 1 << player.GroundedLayer, QueryTriggerInteraction.UseGlobal);

        bool sphereTest = Physics.CheckSphere(player.transform.position, player.GroundedRadius, 1 << player.GroundedLayer);

        groundWasHit = groundWasHit || sphereTest;

        if (!hitGround && groundWasHit)
            OnLanded();

#if UNITY_EDITOR
        if (groundWasHit)
        {
            DebugExtension.DebugWireSphere(player.transform.position, Color.green, player.GroundedRadius);
            Debug.DrawRay(ray.origin, ray.direction * player.GroundedDistance, Color.green);
        }
        else
        {
            DebugExtension.DebugWireSphere(player.transform.position, Color.red, player.GroundedRadius);
            Debug.DrawRay(ray.origin, ray.direction * player.GroundedDistance, Color.red);
        }
#endif
        return groundWasHit;
    }

    #region Event methods
    public void OnPlayerCutMove()
    {
        if (OnPlayerCut != null)
            OnPlayerCut();
    }
    
    private void OnJumped()
    {
        if (OnJump != null)
            OnJump();
    }

    private void OnSecondJumped()
    {
        if (OnSecondJump != null)
            OnSecondJump();
    }

    private void OnLanded()
    {
        if (OnLandedOnGround != null)
            OnLandedOnGround();
    }
    #endregion
}