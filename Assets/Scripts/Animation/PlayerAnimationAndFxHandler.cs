using UnityEngine;

/// <summary>
/// Handles particle and FX spawning and Animator related
/// tasks.
/// </summary>
[RequireComponent(typeof(RigidBodyInput))]
public class PlayerAnimationAndFxHandler : MonoBehaviour
{
    #region Inspector variables
    [FancyHeader("Jump Particles", "Various jump particles")]
    [SerializeField]
    private Vector3 particleSpawnOffset;

    [SerializeField]
    private GameObject firstJumpParticle;

    [SerializeField]
    private GameObject secondJumpParticle;

    [FancyHeader("Run Particles")]
    [SerializeField]
    private float runParticleActivationThreshold = 0.1f;

    [SerializeField]
    private ParticleSystem runningParticle;
    #endregion

    #region Internal Members
    private static readonly string ANIMATOR_RUN = "Run";
    private static readonly string ANIMATOR_JUMPED = "Jumped";
    private static readonly string ANIMATOR_GROUNDED = "Grounded";
    private static readonly string ANIMATOR_DASH = "Dash";


    private RigidBodyInput inputHandler;
    private float originalRunningParticleRate = 0f;
    private float vectorOneMagnitude = Vector2.one.magnitude;
    private ParticleSystem.EmissionModule runningEmission;
    private Vector2 inputVector = Vector2.zero;
    private Animator playerAnimator;
    #endregion

    private void Start ()
    {
        inputHandler = GetComponent<RigidBodyInput>();
        playerAnimator = GetComponent<Animator>();
        SetupRunningParticle();

        // Input Controller events
        inputHandler.InputController.OnJump += HandleFirstJump;
        inputHandler.InputController.OnSecondJump += HandleSecondJump;
        inputHandler.InputController.OnLandedOnGround += HandleGrounded;

        // Dash Controller events
        inputHandler.DashController.OnDashStarted += HandleDash;
    }

    private void Update()
    {
        HandleDirectionalInput();
    }

    private void SetupRunningParticle()
    {
        runningParticle.gameObject.SetActive(true);
        originalRunningParticleRate = runningParticle.emission.rateOverTime.constant;
        runningEmission = runningParticle.emission;
    }

    /// <summary>
    /// Handles directional input information for animation, particles, etc.
    /// </summary>
    private void HandleDirectionalInput()
    {
        float horizontal = inputHandler.InputController.HorizontalInputValue;
        float vertical = inputHandler.InputController.VerticalInputValue;

        FillAnimationData(horizontal, vertical);
        HandleRunningParticles(horizontal, vertical);
    }

    private void FillAnimationData(float horizontal, float vertical)
    {
        // Running
        float runSpeed = Mathf.Max(Mathf.Abs(horizontal), Mathf.Abs(vertical));
        playerAnimator.SetFloat(ANIMATOR_RUN, runSpeed);

        // Grounded
        playerAnimator.SetBool(ANIMATOR_GROUNDED, inputHandler.InputController.IsGrounded);
    }

    private void HandleRunningParticles(float horizontal, float vertical)
    {
        var emission = runningParticle.emission;

        inputVector.Set(horizontal, vertical);

        float t = Mathf.InverseLerp(0f, vectorOneMagnitude, inputVector.magnitude);
        emission.rateOverTime = Mathf.Lerp(0f, originalRunningParticleRate, t);
    }

    private void HandleDash()
    {
        // Animation
        playerAnimator.SetTrigger(ANIMATOR_DASH);
    }

    private void HandleFirstJump()
    {
        // Particles
        runningEmission.enabled = false;
        if (firstJumpParticle != null)
            Instantiate(firstJumpParticle, transform.position + particleSpawnOffset, firstJumpParticle.transform.rotation);

        // Animation
        playerAnimator.SetTrigger(ANIMATOR_JUMPED);
    }

    private void HandleSecondJump()
    {
        // Particles
        if (secondJumpParticle != null)
            Instantiate(secondJumpParticle, transform.position + particleSpawnOffset, secondJumpParticle.transform.rotation);

        // Animation
        playerAnimator.SetTrigger(ANIMATOR_JUMPED);
    }

    private void HandleGrounded()
    {
        runningEmission.enabled = true;
    }
}