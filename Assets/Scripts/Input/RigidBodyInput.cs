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

    [Header("Dash settings")]
    [SerializeField]
    private float dashForce = 5f;

    [SerializeField]
    [Tooltip("Time of the dash in seconds.")]
    private float dashTime = 0.25f;

    [SerializeField]
    [Tooltip("Time after the next dash can be performed.")]
    private float dashCoolDownTime = 0.7f;

    [SerializeField]
    private float dashGroundCheckDistance = 0.5f;

    [SerializeField]
    [Tooltip("Collision sphere radius during the dash.")]
    private float dashCheckRadius = 0.7f;
    #endregion

    #region Internal Members
    private StandardPlayerAction playerAction;
    private Rigidbody rigid;
    private InputHandler inputHandler = null;
    private DashHandler dashHandler = null;
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
    public InputHandler InputController { get { return inputHandler; } }
    public DashHandler DashController { get { return dashHandler; } }
    public float DashForce { get { return dashForce; } }
    public float DashTime { get { return dashTime; } }
    public float DashCoolDownTime { get { return dashCoolDownTime; } }
    public float DashGroundCheckDistance { get { return dashGroundCheckDistance; } }
    public float DashCheckRadius { get { return dashCheckRadius; } }
    public StandardPlayerAction PlayerAction { get { return playerAction; } set { playerAction = value; } }
    #endregion

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        inputHandler = new RigidBodyInputHandler(this);
        dashHandler = new RigidBodyDashHandler(this);
    }

	private void Update ()
    {
        inputHandler.HandleInput();
        dashHandler.HandleDash();
	}
}