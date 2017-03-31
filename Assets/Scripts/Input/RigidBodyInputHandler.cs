﻿using UnityEngine;

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
    private Camera mainCamera;

    private float horizontalInputValue = 0f;
    private float verticalInputValue = 0f;

    private Vector3 forwardVector = Vector3.forward;
    private Vector3 rightVector = Vector3.right;

    private bool secondJump = false;
    #endregion

    #region Properties
    public bool MovementEnabled
    {
        get { return movementEnabled; }
        set { movementEnabled = value; }
    }

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
    #endregion

    public RigidBodyInputHandler(RigidBodyInput player)
    {
        this.player = player;
        this.mainCamera = CameraUtil.GetMainCamera();
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
        HorizontalInputValue = Input.GetAxis(HORIZONTAL_AXIS);
        VerticalInputValue = Input.GetAxis(VERTICAL_AXIS);
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
        bool hitGround = CheckPlayerGrounded();
        // Normal ground jump
        if (hitGround && Input.GetButtonDown(JUMP_AXIS))
        {
            player.Rigid.AddForce(Vector3.up * player.JumpIntensity, ForceMode.VelocityChange);
            secondJump = false;
        }

        // Second jump
        if (!hitGround && !secondJump && Input.GetButtonDown(JUMP_AXIS))
        {
            player.Rigid.velocity = Vector3.zero;   // :(
            player.Rigid.AddForce(
                Vector3.up * player.JumpIntensity * player.SecondJumpIntensityFactor,
                ForceMode.VelocityChange);

            secondJump = true;
        }
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
            forwardVector = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up);
            rightVector = Vector3.Cross(Vector3.up, forwardVector);
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