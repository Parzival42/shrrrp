using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidBodyInput : MonoBehaviour
{
    #region Public fields
    [Header("Movement Settings")]
    [SerializeField]
    private float movementSpeed = 0.1f;

    [SerializeField]
    private float rotationSpeed = 10f;

    [SerializeField]
    private bool cameraBasedControl = false;

    [Header("Jump Settings")]
    [SerializeField]
    private float jumpIntensity = 1f;

    [SerializeField]
    [Tooltip("Decrease factor of the second jump.")]
    private float secondJumpIntensityFactor = 1f;

    [SerializeField]
    [Tooltip("Only if the player is under this height above the ground, he is able to jump.")]
    private float groundedDistance = 0.1f;

    [SerializeField]
    [Tooltip("The offset of the pivot (Since the ground check start from the pivot point).")]
    private Vector3 groundPivotOffset = Vector3.zero;

    [SerializeField]
    [Tooltip("Collision layer of the ground check.")]
    private int groundedLayer = 8;
    #endregion

    #region Internal Members
    private Rigidbody rigid;
    private InputHandler inputHandler = null;
    #endregion

    #region Properties
    public Rigidbody Rigid { get { return rigid; } }
    public float MovementSpeed { get { return movementSpeed; } }
    public float RotationSpeed { get { return rotationSpeed; } }
    public float JumpIntensity { get { return jumpIntensity; } }
    public float GroundedDistance { get { return groundedDistance; } }
    public int GroundedLayer { get { return groundedLayer; } }
    public Vector3 GroundPivotOffset { get { return groundPivotOffset; } }
    public bool CameraBasedControl { get { return cameraBasedControl; } }
    public float SecondJumpIntensityFactor { get { return secondJumpIntensityFactor; } }
    #endregion

    private void Start ()
    {
        rigid = GetComponent<Rigidbody>();
        inputHandler = new RigidBodyInputHandler(this);
	}
	
	private void Update ()
    {
        inputHandler.HandleInput();
	}
}