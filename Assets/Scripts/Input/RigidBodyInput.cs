using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class RigidBodyInput : MonoBehaviour
{
    #region Public fields
    [FancyHeader("Movement Settings", "Speed and general stuff")]
    [SerializeField]
    private float movementSpeed = 0.1f;

    [SerializeField]
    private float rotationSpeed = 10f;

    [SerializeField]
    private bool allowMovement = true;

    [SerializeField]
    private bool cameraBasedControl = false;

    [FancyHeader("Jump Settings", "Jump and second jump")]
    [SerializeField]
    private float jumpIntensity = 1f;

    [SerializeField]
    [Tooltip("Decrease factor of the second jump.")]
    private float secondJumpIntensityFactor = 1f;

    [SerializeField]
    [Tooltip("Only if the player is under this height above the ground, he is able to jump.")]
    private float groundedDistance = 0.1f;

    [SerializeField]
    [Tooltip("Additional radial check.")]
    private float groundedRadius = 0.3f;

    [SerializeField]
    [Tooltip("The offset of the pivot (Since the ground check start from the pivot point).")]
    private Vector3 groundPivotOffset = Vector3.zero;

    [SerializeField]
    [Tooltip("Collision layer of the ground check.")]
    private int groundedLayer = 8;

    [FancyHeader("Dash Settings")]
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
    private float dashCollisionForwardOffset = 0.7f;

    [SerializeField]
    [Tooltip("Collision sphere radius during the dash. (For player)")]
    private float dashCheckRadius = 0.7f;

    [SerializeField]
    [Tooltip("Collision sphere radius for ground.")]
    private float dashGroundCheckRadius = 0.3f;

    [SerializeField]
    [Tooltip("The other player will be pushed back by this force.")]
    private float dashPushBackForce = 10f;
    #endregion

    #region Internal Members
    private static readonly string PLAYER_CANVAS_TAG = "MenuEnvironment";
    private static readonly string PLAYER_LIFE_TEXT_TAG = "HealthText";
    private StandardPlayerAction playerAction;
    private Rigidbody rigid;
    private InputHandler inputHandler = null;
    private DashHandler dashHandler = null;

    private Canvas playerCanvas;
    private Text playerLifeText;
    private Player playerComponent;
    #endregion

    #region Properties
    public Rigidbody Rigid { get { return rigid; } }
    public float MovementSpeed { get { return movementSpeed; } }
    public float RotationSpeed { get { return rotationSpeed; } }
    public bool AllowMovement { get { return allowMovement; } set { allowMovement = value; } }
    public float JumpIntensity { get { return jumpIntensity; } }
    public float GroundedDistance { get { return groundedDistance; } }
    public float GroundedRadius { get { return groundedRadius; } }
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
    public float DashCollisionForwardOffset { get { return dashCollisionForwardOffset; } }
    public float DashCheckRadius { get { return dashCheckRadius; } }
    public float DashGroundCheckRadius { get { return dashGroundCheckRadius; } }
    public float DashPushBackForce { get { return dashPushBackForce; } }
    public StandardPlayerAction PlayerAction { get { return playerAction; } set { SetPlayerAction(value); } }
    #endregion

    private void Awake()
    {
        playerAction = StandardPlayerAction.CreateNullBinding();
        rigid = GetComponent<Rigidbody>();
        inputHandler = new RigidBodyInputHandler(this);
        dashHandler = new RigidBodyDashHandler(this);
        playerComponent = GetComponent<Player>();
    }

    private void Start()
    {
        playerCanvas = gameObject.FindComponentInChildWithTag<Canvas>(PLAYER_CANVAS_TAG);
        playerLifeText = playerCanvas.gameObject.FindComponentInChildWithTag<Text>(PLAYER_LIFE_TEXT_TAG);

        if(FindObjectOfType<GameManager>() != null)
            StartSpawnTween();
    }

    public void StartSpawnTween()
    {
        playerLifeText.text = playerComponent.PlayerLives.ToString();
        // Life tween
        LeanTween.value(0f, 1f, 0.5f).setEase(LeanTweenType.easeInOutSine)
            .setOnUpdate(SetPlayerLifeTextAlpha)
            .setOnComplete(() => {
                LeanTween.value(1f, 0f, 0.5f).setEase(LeanTweenType.easeInOutSine)
                .setOnUpdate(SetPlayerLifeTextAlpha)
                .setDelay(1.5f);
            });

        // Scale tween
        transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack);

        // Position tween (Falling down)
        float originalY = transform.position.y;
        transform.position = new Vector3(transform.position.x, transform.position.y + 6f, transform.position.z);
        LeanTween.moveY(gameObject, originalY, 0.25f).setEase(LeanTweenType.easeOutQuad);
    }

    private void Update()
    {
        dashHandler.HandleDash();
        inputHandler.HandleInput();
    }

    private void SetPlayerAction(StandardPlayerAction newPlayerAction)
    {
        if (playerAction != null)
        {
            playerAction.Destroy();
            playerAction = newPlayerAction;
        }
    }

    private void SetPlayerLifeTextAlpha(float alpha)
    {
        playerLifeText.color = new Color(playerLifeText.color.r, playerLifeText.color.g, playerLifeText.color.b, alpha);
    }
}